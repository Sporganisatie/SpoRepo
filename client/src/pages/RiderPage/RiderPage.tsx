import { useParams } from "react-router-dom";
import { useRiderPage } from "./RiderPageHook";

const RiderPage = () => {
  let { riderId } = useParams();
  const riderInfo = useRiderPage(riderId);
  var fullName = `${riderInfo?.rider?.firstname} ${riderInfo?.rider?.lastname}`;
  document.title = fullName;


  return (
    <div>
      {fullName} heeft meegedaan aan:
      {riderInfo?.riderParticipations.map((element: any) => (
        <div key={element.race.raceId}>{`${element.race.name} ${element.race.year}`}</div>
      ))}
    </div>
  );
};

export default RiderPage;
