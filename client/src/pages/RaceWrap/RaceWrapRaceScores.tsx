import { useEffect, useState } from "react";
import SreDataTable from "../../components/shared/SreDataTable";
import { RaceScoreData } from "./RaceWrapHook";

type RaceScore = RaceScoreData & { rank: number; username?: string };

const RaceWrapRaceScores = ({
    raceScoreData,
    raceId,
}: {
    raceScoreData: RaceScoreData[];
    raceId: number;
}) => {
    const [topScoreRanking, setTopscoreRanking] = useState<RaceScore[]>([]);
    const [bottomScore, setBottomScoreRanking] = useState<RaceScore[]>([]);
    const [averageScore, setAverageScoreRanking] = useState<RaceScore[]>([]);
    const [greatestScore, setGreatestScoreRanking] = useState<
        (RaceScore & { diff: number })[]
    >([]);

    useEffect(() => {
        setTopscoreRanking(
            raceScoreData
                .slice()
                .sort((a, b) => b.topScore - a.topScore)
                .map((data, idx) => {
                    return {
                        ...data,
                        rank: idx + 1,
                        username: data.usernamesAndScores[0].username,
                    };
                })
                .filter(
                    (data) =>
                        data.rank < 6 ||
                        data.rank > raceScoreData.length - 5 ||
                        data.raceId === raceId
                )
        );
        setBottomScoreRanking(
            raceScoreData
                .slice()
                .sort((a, b) => a.bottomScore - b.bottomScore)
                .map((data, idx) => {
                    return {
                        ...data,
                        rank: idx + 1,
                        username:
                            data.usernamesAndScores[
                                data.usernamesAndScores.length - 1
                            ].username,
                    };
                })
                .filter(
                    (data) =>
                        data.rank < 6 ||
                        data.rank > raceScoreData.length - 5 ||
                        data.raceId === raceId
                )
        );
        setAverageScoreRanking(
            raceScoreData
                .slice()
                .sort((a, b) => b.average - a.average)
                .map((data, idx) => {
                    return {
                        ...data,
                        rank: idx + 1,
                    };
                })
                .filter(
                    (data) =>
                        data.rank < 6 ||
                        data.rank > raceScoreData.length - 5 ||
                        data.raceId === raceId
                )
        );
        setGreatestScoreRanking(
            raceScoreData
                .slice()
                .map((scores) => {
                    return {
                        ...scores,
                        diff: scores.topScore - scores.average,
                    };
                })
                .sort((a, b) => b.diff - a.diff)
                .map((data, idx) => {
                    return {
                        ...data,
                        rank: idx + 1,
                        username: data.usernamesAndScores[0].username,
                    };
                })
                .filter(
                    (data) =>
                        data.rank < 6 ||
                        data.rank > raceScoreData.length - 5 ||
                        data.raceId === raceId
                )
        );
    }, [raceScoreData, raceId]);

    return (
        <div
            style={{
                display: "flex",
                gap: "1rem",
                flexWrap: "wrap",
                justifyContent: "center",
            }}>
            <SreDataTable
                title="Topscore per race"
                data={topScoreRanking}
                columns={[
                    {
                        name: "",
                        selector: (row) => row.rank,
                    },
                    {
                        name: "Naam",
                        selector: (row) => row.username ?? "",
                    },
                    {
                        name: "Race",
                        selector: (row) => row.name + " " + row.year,
                    },
                    {
                        name: "Score",
                        selector: (row) => row.topScore,
                    },
                ]}
                conditionalRowStyles={[
                    {
                        when: (row) => row.raceId === raceId,
                        style: {
                            fontWeight: "bold",
                            color: "gold",
                        },
                    },
                ]}
            />
            <SreDataTable
                title="Gemiddelde per race"
                data={averageScore}
                columns={[
                    {
                        name: "",
                        selector: (row) => row.rank,
                    },
                    {
                        name: "Race",
                        selector: (row) => row.name + " " + row.year,
                    },
                    {
                        name: "Score",
                        selector: (row) => row.average,
                    },
                ]}
                conditionalRowStyles={[
                    {
                        when: (row) => row.raceId === raceId,
                        style: {
                            fontWeight: "bold",
                            color: "gold",
                        },
                    },
                ]}
            />
            <SreDataTable
                title="Grootste overwinning"
                data={greatestScore}
                columns={[
                    {
                        name: "",
                        selector: (row) => row.rank,
                    },
                    {
                        name: "Naam",
                        selector: (row) => row.username ?? "",
                    },
                    {
                        name: "Race",
                        selector: (row) => row.name + " " + row.year,
                    },
                    {
                        name: "Score",
                        selector: (row) => row.diff,
                    },
                ]}
                conditionalRowStyles={[
                    {
                        when: (row) => row.raceId === raceId,
                        style: {
                            fontWeight: "bold",
                            color: "gold",
                        },
                    },
                ]}
            />{" "}
            <SreDataTable
                title="Laagste score per race"
                data={bottomScore}
                columns={[
                    {
                        name: "",
                        selector: (row) => row.rank,
                    },
                    {
                        name: "Naam",
                        selector: (row) => row.username ?? "",
                    },
                    {
                        name: "Race",
                        selector: (row) => row.name + " " + row.year,
                    },
                    {
                        name: "Score",
                        selector: (row) => row.bottomScore,
                    },
                ]}
                conditionalRowStyles={[
                    {
                        when: (row) => row.raceId === raceId,
                        style: {
                            fontWeight: "bold",
                            color: "gold",
                        },
                    },
                ]}
            />
        </div>
    );
};

export default RaceWrapRaceScores;
