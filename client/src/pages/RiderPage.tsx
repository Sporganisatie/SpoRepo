import { useParams } from "react-router-dom";

const RiderPage = () => {
  let { riderId } = useParams();
  document.title = "Renner " + riderId
  return (
    <div>
      Hier komt info over de renner waar je op geklikt hebt.
      Suggesties worden gewaardeerd, maar niet perse gehonoreerd.
    </div>
  );
};

export default RiderPage;
