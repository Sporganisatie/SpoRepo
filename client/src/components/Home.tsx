import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useRaceContext, useRaceDispatch } from "./shared/RaceContextProvider";
import axios from "axios";

const Home = () => {
  const navigate = useNavigate();
  const race = useRaceContext();
  const raceDispatch = useRaceDispatch();

  useEffect(() => {
    if (race === 0) {
      axios
        .get(`/api/Race/current`)
        .then((res) => {
          raceDispatch(res.data)
          navigate(`/race/${res.data}`);
        })
    }
    else {
      navigate(`/race/${race}`);
    }
  }, [navigate, race, raceDispatch]);

  return <div>Loading...</div>;
};

export default Home;
