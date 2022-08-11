import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div>
      ik ben Home
      <Link to="/login">klik hier om in te loggen</Link>
    </div>
  );
};

export default Home;
