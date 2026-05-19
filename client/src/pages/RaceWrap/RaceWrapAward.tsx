const RaceWrapAward = ({
  awardName,
  awardWinners,
}: {
  awardName: string;
  awardWinners: string;
}) => {
  return (
    <div className="center-stack" style={{ marginTop: "4rem", marginBottom: "2rem" }}>
      <div className="center-stack" style={{ gap: "0.5rem", fontSize: "2.5rem" }}>
        <div>{awardName}</div>
        <div className="divider-line" />
        <div>{awardWinners}</div>
      </div>
    </div>
  );
};

export default RaceWrapAward;
