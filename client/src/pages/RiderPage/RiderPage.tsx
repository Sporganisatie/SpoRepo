import { useParams } from "react-router-dom";
import { useRiderPage } from "./RiderPageHook";

const RiderPage = () => {
  let { riderId } = useParams();
  const rider = useRiderPage(riderId);
  var fullName = `${rider?.firstname} ${rider?.lastname}`;
  document.title = fullName;


  return (
    <div>
      {fullName} heeft meegedaan aan:
      {rider?.riderParticipations.map((element: any) => (
        <div key={element.race.raceId}>{`${element.race.name} ${element.race.year}`}</div>
      ))}
    </div>
  );
};

export default RiderPage;
