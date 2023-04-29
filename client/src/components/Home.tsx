import { useEffect } from "react";
import { useNavigate } from "react-router-dom";

const Home = () => {
  const navigate = useNavigate();

  useEffect(() => {
    navigate("/race/27");
  }, []);

  return (
    <div>
      Empty home page
      Dit zou je niet moeten kunnen zien. Val Arjen lastig als je dit wel ziet.
    </div>
  );
};

export default Home;
