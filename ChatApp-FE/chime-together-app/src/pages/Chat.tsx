import { useState, useEffect, useRef } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useNavigate } from "react-router-dom";
import { MessageSquare } from "lucide-react";
import { toast } from "@/hooks/use-toast";
import { apiRequest, handleApiResponse, getCurrentUser } from "@/lib/api";

interface User {
  id: string;
  username: string;
  email: string;
}

interface ChatMessage {
  _id: string;
  conversationId: string;
  senderId: string;
  receiverId: string;
  content: string;
  sentAt: string; // ISO string
}

const Chat = () => {
  const navigate = useNavigate();

  const [users, setUsers] = useState<User[]>([]);
  const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [currentUser, setCurrentUser] = useState<User | null>(null);

  const [searchQuery, setSearchQuery] = useState("");
  const [isLoading, setIsLoading] = useState(true);

  const [message, setMessage] = useState("");
  const [isSending, setIsSending] = useState(false);

  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [conversationMap, setConversationMap] = useState<Record<string, string>>({});

  const chatRef = useRef<HTMLDivElement>(null);

  // Auto-scroll chat
  useEffect(() => {
    chatRef.current?.scrollTo({
      top: chatRef.current.scrollHeight,
      behavior: "smooth",
    });
  }, [messages]);

  // Load users + current user
  useEffect(() => {
    fetchUsers();
  }, []);

  // Search logic
  useEffect(() => {
    const delay = setTimeout(() => {
      if (searchQuery.trim()) {
        searchUsers(searchQuery);
      } else {
        setFilteredUsers(users.filter(u => u.id !== currentUser?.id));
      }
    }, 400);

    return () => clearTimeout(delay);
  }, [searchQuery, users, currentUser]);

  const fetchUsers = async () => {
    try {
      const res = await apiRequest("/Users");
      const list = await handleApiResponse<User[]>(res);

      const logged = getCurrentUser();
      const current = list.find(u => u.username.toLowerCase() === logged.username.toLowerCase());

      setCurrentUser(current || null);
      setUsers(list);
      setFilteredUsers(list.filter(u => u.id !== current?.id));
    } catch {
      toast({ title: "Unable to load users", variant: "destructive" });
    } finally {
      setIsLoading(false);
    }
  };

  const searchUsers = async (name: string) => {
    try {
      const res = await apiRequest(`/Users/search?userName=${name}`);
      const list = await handleApiResponse<User[]>(res);
      setFilteredUsers(list.filter(u => u.id !== currentUser?.id));
    } catch {
      setFilteredUsers([]);
    }
  };

  const loadMessages = async () => {
    if (!selectedUser || !currentUser) return;

    let convId = conversationMap[selectedUser.id];

    // If no conversation yet, attempt to create one via API (optional)
    if (!convId) {
      setMessages([]);
      return;
    }

    try {
      const res = await apiRequest(`/Chat/history?conversationId=${convId}`);
      const list = await handleApiResponse<ChatMessage[]>(res);
      setMessages(list);
    } catch {
      toast({ title: "Error loading messages", variant: "destructive" });
    }
  };

  // Refresh chat every 3 seconds
  useEffect(() => {
    if (!selectedUser) return;

    loadMessages();
    const interval = setInterval(loadMessages, 3000);
    return () => clearInterval(interval);
  }, [selectedUser, conversationMap]);

  const openChat = (user: User) => {
    setSelectedUser(user);
    loadMessages();
  };

  const sendMessage = async () => {
    if (!message.trim() || !currentUser || !selectedUser) return;

    setIsSending(true);

    try {
      const res = await apiRequest("/Chat/send", {
        method: "POST",
        body: JSON.stringify({
          senderId: currentUser.id,
          receiverId: selectedUser.id,
          content: message.trim(),
        }),
      });

      const data = await handleApiResponse<{ conversationId: string }>(res);

      setConversationMap(prev => ({
        ...prev,
        [selectedUser.id]: data.conversationId,
      }));

      setMessage("");
      loadMessages();
    } catch {
      toast({ title: "Failed to send message", variant: "destructive" });
    } finally {
      setIsSending(false);
    }
  };

  return (
    <div className="flex min-h-screen bg-background">

      {/* Sidebar */}
      <div className="w-80 border-r p-4">
        <h2 className="text-lg font-bold mb-4">Users</h2>

        <Input
          placeholder="Search users..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
        />

        <div className="mt-4 space-y-2">
          {filteredUsers.map(user => (
            <div
              key={user.id}
              onClick={() => openChat(user)}
              className={`p-3 rounded cursor-pointer ${
                selectedUser?.id === user.id ? "bg-primary/20" : "hover:bg-muted"
              }`}
            >
              <div className="font-medium">{user.username}</div>
              <div className="text-sm text-muted-foreground">{user.email}</div>
            </div>
          ))}
        </div>
      </div>

      {/* Chat window */}
      <div className="flex-1 flex flex-col">
        {!selectedUser ? (
          <div className="flex-1 flex items-center justify-center text-muted-foreground">
            Select a user to start chatting
          </div>
        ) : (
          <>
            <div className="border-b p-4">
              <h2 className="text-xl font-semibold">{selectedUser.username}</h2>
            </div>

            <div ref={chatRef} className="flex-1 p-4 overflow-y-auto">
              {messages.map(msg => (
                <div
                  key={msg._id}
                  className={`max-w-xs mb-3 p-3 rounded-xl ${
                    msg.senderId === currentUser?.id
                      ? "ml-auto bg-primary text-primary-foreground"
                      : "bg-muted"
                  }`}
                >
                  {msg.content}
                  <div className="text-xs mt-1 opacity-80">
                    {new Date(msg.sentAt).toLocaleTimeString()}
                  </div>
                </div>
              ))}
            </div>

            <div className="p-4 border-t flex gap-2">
              <Input
                placeholder="Type a message..."
                value={message}
                onChange={(e) => setMessage(e.target.value)}
                onKeyDown={(e) => {
                  if (e.key === "Enter") sendMessage();
                }}
              />
              <Button onClick={sendMessage} disabled={isSending || !message.trim()}>
                {isSending ? "Sending..." : "Send"}
              </Button>
            </div>
          </>
        )}
      </div>
    </div>
  );
};

export default Chat;
