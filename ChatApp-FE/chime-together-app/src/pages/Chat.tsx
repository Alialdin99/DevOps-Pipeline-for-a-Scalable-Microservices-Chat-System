import { useState, useEffect, useRef } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { useNavigate } from "react-router-dom";
import { MessageSquare, Search, LogOut, User, Bell, X } from "lucide-react";
import { toast } from "@/hooks/use-toast";
import { apiRequest, handleApiResponse, getCurrentUser } from "@/lib/api";

interface User {
  id: string;
  username: string;
  email: string;
}

interface Notification {
  id: string;
  fromUser: string;
  message: string;
  timestamp: Date;
}

const Chat = () => {
  const navigate = useNavigate();
  const [users, setUsers] = useState<User[]>([]);
  const [filteredUsers, setFilteredUsers] = useState<User[]>([]);
  const [searchQuery, setSearchQuery] = useState("");
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [currentUsername, setCurrentUsername] = useState<string>("");
  const [currentUser, setCurrentUser] = useState<User | null>(null);
  const [notifications, setNotifications] = useState<Notification[]>([]);
  const [showNotifications, setShowNotifications] = useState(false);
  const [message, setMessage] = useState("");
  const [isSending, setIsSending] = useState(false);
  const [chatTheme, setChatTheme] = useState<string>(
    localStorage.getItem('chatTheme') || 'default'
  );
  const [chatBackground, setChatBackground] = useState<string>(
    localStorage.getItem('chatBackground') || ''
  );
  const notificationRef = useRef<HTMLDivElement>(null);

  // Update theme when it changes in localStorage
  useEffect(() => {
    const handleStorageChange = () => {
      setChatTheme(localStorage.getItem('chatTheme') || 'default');
      setChatBackground(localStorage.getItem('chatBackground') || '');
    };
    window.addEventListener('storage', handleStorageChange);
    return () => window.removeEventListener('storage', handleStorageChange);
  }, []);

  useEffect(() => {
    // Get current user info
    const user = getCurrentUser();
    if (user) {
      setCurrentUsername(user.username.toLowerCase());
    }
    fetchAllUsers();
    
    // Poll for notifications every 5 seconds
    const notificationInterval = setInterval(fetchNotifications, 5000);
    fetchNotifications();
    
    return () => clearInterval(notificationInterval);
  }, []);

  // Clear message when switching users
  useEffect(() => {
    setMessage("");
  }, [selectedUser]);

  // Close notification dropdown when clicking outside
  useEffect(() => {
    const handleClickOutside = (event: MouseEvent) => {
      if (notificationRef.current && !notificationRef.current.contains(event.target as Node)) {
        setShowNotifications(false);
      }
    };

    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Debounced search effect - 500ms delay
  useEffect(() => {
    const delayDebounce = setTimeout(() => {
      if (searchQuery.trim()) {
        searchUsers(searchQuery);
      } else {
        // Show all users except current user when search is empty
        const filtered = users.filter(
          user => user.username.toLowerCase() !== currentUsername
        );
        setFilteredUsers(filtered);
      }
    }, 500);

    return () => clearTimeout(delayDebounce);
  }, [searchQuery, currentUsername]);

  const fetchAllUsers = async () => {
    try {
      const response = await apiRequest('/Users');
      const data = await handleApiResponse<User[]>(response);
      
      // Find current user in the list
      const user = getCurrentUser();
      const current = data.find(u => u.username.toLowerCase() === user?.username.toLowerCase());
      if (current) {
        setCurrentUser(current);
      }
      
      // Filter out current user from the list
      const filtered = data.filter(
        user => user.username.toLowerCase() !== currentUsername
      );
      
      setUsers(data);
      setFilteredUsers(filtered);
    } catch (error) {
      toast({ 
        title: "Unable to load users", 
        description: "Please check your connection and try again.",
        variant: "destructive"
      });
    } finally {
      setIsLoading(false);
    }
  };

  const searchUsers = async (userName: string) => {
    try {
      const response = await apiRequest(
        `/Users/search?userName=${encodeURIComponent(userName)}`
      );
      const data = await handleApiResponse<User[]>(response);
      
      // Filter out current user from search results
      const filtered = (Array.isArray(data) ? data : [data]).filter(
        user => user.username.toLowerCase() !== currentUsername
      );
      
      setFilteredUsers(filtered);
    } catch (error) {
      // Silent fail for search - just show no results
      setFilteredUsers([]);
    }
  };



  const fetchNotifications = async () => {
    try {
      // Your friend will implement this endpoint
      const response = await apiRequest('/Notifications');
      const data = await handleApiResponse<Notification[]>(response);
      setNotifications(data || []);
    } catch (error) {
      // Silent fail - notifications are not critical
      console.log('Could not fetch notifications');
    }
  };

  const deleteNotification = async (notificationId: string) => {
    try {
      await apiRequest(`/Notifications/${notificationId}`, { method: 'DELETE' });
      setNotifications(notifications.filter(n => n.id !== notificationId));
      toast({ title: "Notification deleted" });
    } catch (error) {
      toast({ 
        title: "Could not delete notification",
        variant: "destructive"
      });
    }
  };

  const viewNotificationMessage = (notification: Notification) => {
    // Find the user who sent the message
    const user = users.find(u => u.username.toLowerCase() === notification.fromUser.toLowerCase());
    if (user) {
      setSelectedUser(user);
      setShowNotifications(false);
    }
  };

  const sendMessage = async () => {
    if (!message.trim() || !selectedUser || !currentUser) {
      return;
    }

    setIsSending(true);

    try {
      const response = await apiRequest('/Chat/send', {
        method: 'POST',
        body: JSON.stringify({
          senderId: currentUser.id,
          receiverId: selectedUser.id,
          content: message.trim()
        })
      });

      await handleApiResponse(response);
      
      toast({ title: "Message sent!" });
      setMessage(""); // Clear message after sending
    } catch (error) {
      toast({
        title: "Failed to send message",
        description: error instanceof Error ? error.message : "Unable to send message. Please try again.",
        variant: "destructive"
      });
    } finally {
      setIsSending(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    toast({ title: "Logged out successfully" });
    navigate("/login");
  };

  return (
    <div className="min-h-screen flex bg-background">
      {/* Sidebar */}
      <div className="w-80 border-r border-border flex flex-col bg-card">
        {/* Sidebar Header */}
        <div className="p-4 border-b border-border">
          <div className="flex items-center justify-between mb-4">
            <div className="flex items-center gap-2">
              <div className="w-10 h-10 bg-gradient-to-br from-primary to-accent rounded-xl flex items-center justify-center">
                <MessageSquare className="w-5 h-5 text-primary-foreground" />
              </div>
              <div>
                <h1 className="text-xl font-bold bg-gradient-to-r from-primary to-accent bg-clip-text text-transparent">
                  Chat
                </h1>
                {currentUsername && (
                  <p className="text-sm text-muted-foreground">Hello, {currentUsername}</p>
                )}
              </div>
            </div>
            <div className="flex gap-2">
              <div className="relative" ref={notificationRef}>
                <Button
                  onClick={() => setShowNotifications(!showNotifications)}
                  variant="outline"
                  size="icon"
                  title="Notifications"
                  className="relative"
                >
                  <Bell className="w-5 h-5" />
                  {notifications.length > 0 && (
                    <span className="absolute -top-1 -right-1 bg-destructive text-destructive-foreground text-xs rounded-full w-5 h-5 flex items-center justify-center font-bold">
                      {notifications.length}
                    </span>
                  )}
                </Button>
                
                {showNotifications && (
                  <div className="absolute right-0 mt-2 w-80 bg-card border border-border rounded-lg shadow-lg z-50 max-h-96 overflow-y-auto">
                    <div className="p-3 border-b border-border">
                      <h3 className="font-semibold text-foreground">Notifications</h3>
                    </div>
                    {notifications.length === 0 ? (
                      <div className="p-4 text-center text-muted-foreground">
                        No new notifications
                      </div>
                    ) : (
                      <div className="divide-y divide-border">
                        {notifications.map((notification) => (
                          <div
                            key={notification.id}
                            className="p-3 hover:bg-accent/50 transition-colors"
                          >
                            <div className="flex items-start justify-between gap-2">
                              <div className="flex-1 cursor-pointer" onClick={() => viewNotificationMessage(notification)}>
                                <p className="font-medium text-sm text-foreground">
                                  {notification.fromUser}
                                </p>
                                <p className="text-sm text-muted-foreground truncate">
                                  {notification.message}
                                </p>
                                <p className="text-xs text-muted-foreground mt-1">
                                  {new Date(notification.timestamp).toLocaleString()}
                                </p>
                              </div>
                              <Button
                                onClick={() => deleteNotification(notification.id)}
                                variant="ghost"
                                size="icon"
                                className="h-6 w-6 flex-shrink-0"
                              >
                                <X className="w-4 h-4" />
                              </Button>
                            </div>
                          </div>
                        ))}
                      </div>
                    )}
                  </div>
                )}
              </div>
              <Button
                onClick={() => navigate("/profile")}
                variant="outline"
                size="icon"
                title="Profile"
              >
                <User className="w-5 h-5" />
              </Button>
              <Button
                onClick={handleLogout}
                variant="outline"
                size="icon"
                title="Logout"
              >
                <LogOut className="w-5 h-5" />
              </Button>
            </div>
          </div>
          
          {/* Search */}
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 w-4 h-4 text-muted-foreground" />
            <Input
              placeholder="Search users..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className="pl-9"
            />
          </div>
        </div>

        {/* User List */}
        <div className="flex-1 overflow-y-auto">
          {isLoading ? (
            <div className="p-4 text-center text-muted-foreground">
              Loading users...
            </div>
          ) : filteredUsers.length === 0 ? (
            <div className="p-4 text-center text-muted-foreground">
              No users found
            </div>
          ) : (
            <div className="p-2">
              {filteredUsers.map((user) => (
                <button
                  key={user.id}
                  onClick={() => setSelectedUser(user)}
                  className={`w-full p-3 rounded-lg text-left transition-colors mb-1 ${
                    selectedUser?.id === user.id
                      ? "bg-primary/10 border border-primary/20"
                      : "hover:bg-accent/50"
                  }`}
                >
                  <div className="font-medium text-foreground">{user.username}</div>
                  <div className="text-sm text-muted-foreground truncate">{user.email}</div>
                </button>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Main Chat Area */}
      <div className="flex-1 flex flex-col">
        {selectedUser ? (
          <>
            {/* Chat Header */}
            <div className="p-4 border-b border-border bg-card">
              <h2 className="text-xl font-semibold">{selectedUser.username}</h2>
              <p className="text-sm text-muted-foreground">{selectedUser.email}</p>
            </div>

            {/* Messages Area */}
            <div 
              className={`flex-1 p-4 overflow-y-auto ${chatTheme === 'dark' ? 'bg-gray-900' : chatTheme === 'light' ? 'bg-gray-50' : chatTheme === 'blue' ? 'bg-blue-50' : chatTheme === 'green' ? 'bg-green-50' : ''}`}
              style={chatBackground ? { 
                backgroundImage: `url(${chatBackground})`, 
                backgroundSize: 'cover', 
                backgroundPosition: 'center',
                backgroundAttachment: 'fixed'
              } : {}}
            >
              <div className="text-center text-muted-foreground">
                Chat with {selectedUser.username} will appear here
              </div>
            </div>

            {/* Message Input */}
            <div className="p-4 border-t border-border bg-card">
              <div className="flex gap-2">
                <Input 
                  placeholder="Type a message..." 
                  className="flex-1"
                  value={message}
                  onChange={(e) => setMessage(e.target.value)}
                  onKeyPress={handleKeyPress}
                  disabled={isSending}
                />
                <Button 
                  className="bg-gradient-to-r from-primary to-accent"
                  onClick={sendMessage}
                  disabled={isSending || !message.trim()}
                >
                  {isSending ? "Sending..." : "Send"}
                </Button>
              </div>
            </div>
          </>
        ) : (
          <div className="flex-1 flex items-center justify-center">
            <div className="text-center space-y-4">
              <div className="mx-auto w-20 h-20 bg-gradient-to-br from-primary to-accent rounded-2xl flex items-center justify-center shadow-lg">
                <MessageSquare className="w-10 h-10 text-primary-foreground" />
              </div>
              <h2 className="text-2xl font-bold text-foreground">Select a user to start chatting</h2>
              <p className="text-muted-foreground">Choose someone from the sidebar to begin</p>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default Chat;
