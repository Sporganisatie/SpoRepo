import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div>
      ik ben Home
      <Link to="/login">klik hier om in te loggen</Link>
      <br />
      <Link to="/testing">Test pagina</Link>
    </div>
  );
};

export default Home;
