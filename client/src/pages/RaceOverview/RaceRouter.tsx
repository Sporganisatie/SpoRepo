import axios from "axios";
import { useEffect } from "react";
import { useParams } from "react-router-dom";
import { useNavigate } from "react-router-dom";

enum RaceStateEnum {
  None,
  NotJoined,
  TeamSelection,
  Started,
  Finished,
}

// TODO dit component vervangen door pure typescript
const RaceRouter = () => {
  let navigate = useNavigate();
  let { raceId } = useParams();
  useEffect(() => {
    axios
      .get(`/api/Race`, { params: { raceId } })
      .then((res) => {
        switch (res.data.state) {
          case RaceStateEnum.NotJoined:
            navigate(`/${raceId}/joinrace`);
            return;
          case RaceStateEnum.TeamSelection:
            navigate(`/${raceId}/teamselection`);
            return;
          case RaceStateEnum.Started:
            navigate(`/${raceId}/stage/${res.data.currentStage}`);
            return;
        }
      })
      .catch(function (error) { });
  }, [raceId, navigate]);

  return <div style={{ color: 'white' }}>Loading...</div>;
};

export default RaceRouter;
