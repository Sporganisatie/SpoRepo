import { useEffect, useState } from "react";
import { useRaceContext } from "../../components/shared/RaceContextProvider";
import { MissedPointsTableData } from "../Statistics/MissedPointsHook";
import Uitvallers from "../Statistics/Uitvallers";
import { useRaceWrap } from "./RaceWrapHook";
import RaceWrapMissedPoints from "./RaceWrapMissedPoints";
import RaceWrapRaceScores from "./RaceWrapRaceScores";
import RaceWrapStages from "./RaceWrapStages";
import RaceWrapAward from "./RaceWrapAward";
import axios from "axios";
import { SelectOption } from "../../components/Select";
import { faToiletPaper } from "@fortawesome/free-solid-svg-icons";

export type MissedPointsTotalRow = ReturnType<
    typeof getMissedPointsTotal
>[number];

const RaceWrap = () => {
    const { missedPoints, uitvallers, raceScoreData } = useRaceWrap();
    const raceId = useRaceContext();
    const [totals, setTotals] = useState<MissedPointsTotalRow[]>([]);
    const [pechvogel, setPechvogel] = useState<string>();
    const [pietjePrecies, setPietjePrecies] = useState<string>();
    const [raceOptions, setRaceOptions] = useState<SelectOption<number>[]>([]);

    useEffect(() => {
        axios
            .get(`/api/race/all`)
            .then((res) => {
                setRaceOptions(res.data);
            })
    }, []);

    useEffect(() => {
        const totals = getMissedPointsTotal(missedPoints);
        setTotals(totals);
        const percentages =
            totals?.map((t) => Math.round(t.gemist / t.optimaal * 1000)/10) ?? [];
        const maxPercentage = Math.min(...percentages);
        setPietjePrecies(
            totals
                ?.filter((t) => (Math.round(t.gemist / t.optimaal * 1000)/10) === maxPercentage)
                .map((u) => u.username)
                .join(" & "))
    }, [missedPoints]);

    useEffect(() => {
        const lostBudgets =
            uitvallers?.map((u) => u.uitvallerStagesBudget) ?? [];
        const maxLostBudget = Math.max(...lostBudgets);
        setPechvogel(
            uitvallers
                ?.filter((u) => u.uitvallerStagesBudget === maxLostBudget)
                .map((u) => u.userName)
                .join(" & ")
        );
    }, [uitvallers])

    return (
        <div
            style={{
                display: "flex",
                flexDirection: "column",
                alignItems: "center",
                marginBottom: "4rem",
            }}>
            {totals.length === 0 ? (
                <></>
            ) : (
                <div
                    style={{
                        display: "flex",
                        flexDirection: "column",
                        gap: "0.5rem",
                        alignItems: "center",
                    }}>
                    <div
                        style={{
                            fontSize: "2.5rem",
                        }}>
                        🏆 Podium van de { raceOptions.find((opt) => opt.value === raceId)?.displayValue }
                    </div>
                    <div style={{width: "100%", height:"1px", backgroundColor: "white"}}></div>
                    <div
                        style={{
                            color: "#fde047",
                            fontSize: "2.5rem",
                        }}>
                        {totals.at(0)?.username} {totals.at(0)?.behaald}
                    </div>
                    <div
                        style={{
                            color: "#e2e8f0",
                            fontSize: "2rem",
                        }}>
                        {totals.at(1)?.username} {totals.at(1)?.behaald}
                    </div>
                    <div
                        style={{
                            color: "#fbbf24",
                            fontSize: "1.5rem",
                        }}>
                        {totals.at(2)?.username} {totals.at(2)?.behaald}
                    </div>
                    {totals.slice(3).map((total) => {
                        return (
                            <div style={{ fontSize: "1.25rem" }}>
                                {total.username} {total.behaald}{" "}
                            </div>
                        );
                    })}
                </div>
            )}

            {pietjePrecies ? (
                <RaceWrapAward
                    awardName={"🎯 Beste dagopstellingen"}
                    awardWinners={pietjePrecies}
                />
            ) : (
                <></>
            )}
            {missedPoints ? (
                <RaceWrapMissedPoints missedPoints={totals} />
            ) : (
                <></>
            )}

            {pechvogel ? (
                <RaceWrapAward
                    awardName={"🦆 Pechvogel Prijs"}
                    awardWinners={pechvogel}
                />
            ) : (
                <></>
            )}
            <Uitvallers />
            <RaceWrapStages />
                <RaceWrapAward
                    awardName={"Vergelijking met andere races"}
                    awardWinners={""}
                />
            {raceScoreData ? (
                <RaceWrapRaceScores
                    raceScoreData={raceScoreData}
                    raceId={raceId}
                />
            ) : (
                <></>
            )}
        </div>
    );
};

export default RaceWrap;

function getMissedPointsTotal(missedPoints?: MissedPointsTableData[]) {
    if (missedPoints === undefined) {
        return [];
    }
    const pointTotals = missedPoints
        .map((userData) => {
            const data = userData.data.find(
                ({ etappe }) => etappe === "Totaal"
            );
            if (!data) {
                throw new Error("Missing missedpoints totaal");
            }
            return {
                username: userData.username,
                ...data,
            };
        })
        .sort((a, b) => b.behaald - a.behaald);
    const optimalOrder = pointTotals
        .slice()
        .sort((a: any, b: any) => b.optimaal - a.optimaal);
    return pointTotals.map((total, idx) => {
        return {
            ...total,
            scoreIndex: idx,
            optimalIndex: optimalOrder.findIndex(
                (score: any) => score.username === total.username
            ),
        };
    });
}
