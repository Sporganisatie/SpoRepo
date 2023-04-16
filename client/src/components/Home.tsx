import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div>
      ik ben Home
      <Link to="/login">klik hier om in te loggen</Link>
      <br />
      <Link to="/testing">Check login and DB pagina</Link>
      <br />
      <Link to="/teamselection/27/">Team Selection</Link>
    </div>
  );
};

export default Home;
