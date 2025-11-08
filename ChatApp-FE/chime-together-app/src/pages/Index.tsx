import { Button } from "@/components/ui/button";
import { useNavigate } from "react-router-dom";
import challoLogo from "@/assets/challo-logo.png";

const Index = () => {
  const navigate = useNavigate();

  return (
    <div className="min-h-screen bg-gradient-to-br from-background via-background to-secondary/30">
      <div className="container mx-auto px-4 py-16">
        <div className="max-w-4xl mx-auto text-center space-y-12">
          <div className="space-y-6">
            <div className="mx-auto w-32 h-32 rounded-2xl flex items-center justify-center">
              <img src={challoLogo} alt="Challo Logo" className="w-full h-full object-contain" />
            </div>
            <h1 className="text-5xl md:text-6xl font-bold bg-gradient-to-r from-primary to-accent bg-clip-text text-transparent">
              Challo
            </h1>
            <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
              Where chat meets hello. Connect instantly with anyone, anywhere.
            </p>
          </div>

          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button
              size="lg"
              onClick={() => navigate("/login")}
              className="bg-gradient-to-r from-primary to-accent hover:opacity-90 transition-all shadow-lg text-lg px-8"
            >
              Sign In
            </Button>
            <Button
              size="lg"
              variant="outline"
              onClick={() => navigate("/register")}
              className="text-lg px-8 border-2"
            >
              Create Account
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Index;
