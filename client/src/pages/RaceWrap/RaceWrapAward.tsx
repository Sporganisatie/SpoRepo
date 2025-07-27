const RaceWrapAward = ({
    awardName,
    awardWinners,
}: {
    awardName: string;
    awardWinners: string;
}) => {
    return (
        <div
            style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                marginTop: "4rem",
                marginBottom: "2rem",
            }}>
            <div
                style={{
                    display: "flex",
                    flexDirection: "column",
                    gap: "0.5rem",
                    alignItems: "center",
                    fontSize: "2.5rem",
                }}>
                <div>{awardName}</div>
                <div
                    style={{
                        width: "100%",
                        height: "1px",
                        backgroundColor: "white",
                    }}
                />
                <div>{awardWinners}</div>
            </div>
        </div>
    );
};

export default RaceWrapAward;
