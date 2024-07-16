import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useRaceContext } from "./shared/RaceContextProvider";
import axios from "axios";

const Home = () => {
  const navigate = useNavigate();
  const race = useRaceContext();

  useEffect(() => {
    if (race === 0) {
      axios
        .get(`/api/Race/current`)
        .then((res) => {
          navigate(`/${res.data}/race`);
        })
    }
    else {
      navigate(`/${race}/race`);
    }
  }, [navigate, race]);

  // TODO toon links naar alle races (van een user) als er geen actieve race is
  // of als de user expliciet naar races_overzicht/home navigeert
  return <div style={{ color: 'white' }}>Loading...</div>;
};

export default Home;
