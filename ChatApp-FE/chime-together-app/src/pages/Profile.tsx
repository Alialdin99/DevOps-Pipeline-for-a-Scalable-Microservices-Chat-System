import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { useNavigate } from "react-router-dom";
import { toast } from "@/hooks/use-toast";
import { apiRequest, handleApiResponse, getCurrentUser } from "@/lib/api";
import { ArrowLeft, Trash2 } from "lucide-react";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";

interface UserProfile {
  id: string;
  username: string;
  email: string;
}

const Profile = () => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [chatTheme, setChatTheme] = useState<string>(
    localStorage.getItem('chatTheme') || 'default'
  );
  const [chatBackground, setChatBackground] = useState<string>(
    localStorage.getItem('chatBackground') || ''
  );

  useEffect(() => {
    const user = getCurrentUser();
    if (!user) {
      navigate("/login");
      return;
    }
    fetchProfile();
  }, [navigate]);

  const fetchProfile = async () => {
    try {
      const user = getCurrentUser();
      if (!user?.id) {
        navigate("/login");
        return;
      }
      
      const response = await apiRequest(`/Users/${user.id}`);
      const data = await handleApiResponse<UserProfile>(response);
      setProfile(data);
      setUsername(data.username);
      setEmail(data.email);
    } catch (error) {
      toast({
        title: "Failed to load profile",
        description: "Unable to fetch your profile information.",
        variant: "destructive"
      });
    }
  };

  const handleUpdate = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoading(true);

    try {
      const user = getCurrentUser();
      if (!user?.id) {
        navigate("/login");
        return;
      }

      const response = await apiRequest(`/Users/${user.id}`, {
        method: 'PUT',
        body: JSON.stringify({ username, email })
      });

      await handleApiResponse(response);
      
      // Save theme settings to localStorage
      localStorage.setItem('chatTheme', chatTheme);
      localStorage.setItem('chatBackground', chatBackground);
      
      toast({
        title: "Profile updated",
        description: "Your profile and preferences have been updated successfully."
      });
    } catch (error) {
      toast({
        title: "Update failed",
        description: error instanceof Error ? error.message : "Failed to update profile",
        variant: "destructive"
      });
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async () => {
    try {
      const user = getCurrentUser();
      if (!user?.id) {
        navigate("/login");
        return;
      }

      const response = await apiRequest(`/Users/${user.id}`, {
        method: 'DELETE'
      });

      await handleApiResponse(response);

      localStorage.removeItem('authToken');
      toast({
        title: "Account deleted",
        description: "Your account has been permanently deleted."
      });
      
      navigate("/register");
    } catch (error) {
      toast({
        title: "Delete failed",
        description: error instanceof Error ? error.message : "Failed to delete account",
        variant: "destructive"
      });
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-background p-4">
      <div className="w-full max-w-md space-y-6">
        <div className="flex items-center gap-4">
          <Button
            variant="outline"
            size="icon"
            onClick={() => navigate("/chat")}
          >
            <ArrowLeft className="w-4 h-4" />
          </Button>
          <h1 className="text-3xl font-bold bg-gradient-to-r from-primary to-accent bg-clip-text text-transparent">
            Profile Settings
          </h1>
        </div>

        <form onSubmit={handleUpdate} className="space-y-4 bg-card p-6 rounded-lg border border-border">
          <div className="space-y-2">
            <Label htmlFor="username">Username</Label>
            <Input
              id="username"
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>

          <div className="space-y-2">
            <Label htmlFor="chatTheme">Chat Theme</Label>
            <select
              id="chatTheme"
              value={chatTheme}
              onChange={(e) => setChatTheme(e.target.value)}
              className="w-full px-3 py-2 rounded-md border border-input bg-background text-foreground"
            >
              <option value="default">Default</option>
              <option value="dark">Dark</option>
              <option value="light">Light</option>
              <option value="blue">Ocean Blue</option>
              <option value="green">Forest Green</option>
            </select>
          </div>

          <div className="space-y-2">
            <Label htmlFor="chatBackground">Chat Background URL (optional)</Label>
            <Input
              id="chatBackground"
              type="url"
              value={chatBackground}
              onChange={(e) => setChatBackground(e.target.value)}
              placeholder="Enter image URL for chat background"
            />
            {chatBackground && (
              <p className="text-xs text-muted-foreground">Preview will be applied in chat</p>
            )}
          </div>

          <Button
            type="submit" 
            className="w-full bg-gradient-to-r from-primary to-accent"
            disabled={isLoading}
          >
            {isLoading ? "Updating..." : "Update Profile"}
          </Button>
        </form>

        <AlertDialog>
          <AlertDialogTrigger asChild>
            <Button variant="destructive" className="w-full">
              <Trash2 className="w-4 h-4 mr-2" />
              Delete Account
            </Button>
          </AlertDialogTrigger>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
              <AlertDialogDescription>
                This action cannot be undone. This will permanently delete your account
                and remove all your data from our servers.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancel</AlertDialogCancel>
              <AlertDialogAction onClick={handleDelete} className="bg-destructive text-destructive-foreground">
                Delete Account
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
    </div>
  );
};

export default Profile;
