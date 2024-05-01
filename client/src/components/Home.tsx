import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const Home = () => {
  const navigate = useNavigate();

  useEffect(() => {
    navigate("/race/30");
  }, [navigate]);

  return (
    <div>
      Loading...
    </div>
  );
};

export default Home;
