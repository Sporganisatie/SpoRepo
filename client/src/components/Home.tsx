import { Link } from "react-router-dom";

const Home = () => {
  return (
    <div>
      ik ben Home
      <Link to="/login">klik hier om in te loggen</Link>
      <br />
      <Link to="/testing">Test pagina</Link> Tijdelijk kapot in BE
      <br />
      <Link to="/stage/26/21/">Stage pagina</Link>
      <br />
      <Link to="/stage/26/21?budgetparticipation=true">Stage pagina met budget</Link>
    </div>
  );
};

export default Home;
