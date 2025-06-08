

import { useEffect, useState } from "react";
import {
    MissedPointsTableData,
    useMissedPoints,
} from "../Statistics/MissedPointsHook";
import RankedTable from "./RankedTable";
import SreDataTable from "../../components/shared/SreDataTable";

type MissedPointsTotals = ReturnType<typeof getMissedPointsTotal>;
type MissedPointsTotalRow = MissedPointsTotals[number];

const RaceWrapMissedPoints = () => {
    const { data: missedPoints } = useMissedPoints();
    const [missingPointsRanking, setMissingPointsRanking] = useState<MissedPointsTotals
    >([]);
    const [optimalScoreRanking, setOptimalScoreRanking] = useState<MissedPointsTotals
    >([]);

    useEffect(() => {
        const totalsData = getMissedPointsTotal(missedPoints);
        setMissingPointsRanking(
            totalsData.slice().sort((a: MissedPointsTotalRow, b: MissedPointsTotalRow) => a.gemist - b.gemist)
        );
        setOptimalScoreRanking(
            totalsData.slice().sort((a: MissedPointsTotalRow, b: MissedPointsTotalRow) => a.optimaal - b.optimaal)
        );
    }, [missedPoints]);

    return (
        <div>
            <RankedTable
                data={missingPointsRanking}
                getUsername={(v) => v.username}
                getValue={(v) => v.behaald}
            />
            <SreDataTable title="Gemiste punten" columns={[
                {
                    name: "Naam",
                    selector: (row: MissedPointsTotalRow) => row.username,
                    sortable: true,
                },
                {
                    name: "Gemist",
                    selector: (row: MissedPointsTotalRow) => row.gemist,
                    sortable: true,
                },
                {
                    name: "Percentage",
                    selector: (row: MissedPointsTotalRow) => Math.round(row.gemist / row.optimaal * 1000)/10,
                    sortable: true,
                },
            ]} data={missingPointsRanking} />
            <SreDataTable title="Gemiste punten" columns={[
                {
                    name: "",
                    width: "70px",
                    selector: (row: MissedPointsTotalRow) => {
                        if (row.optimalIndex > row.scoreIndex) {
                            return "▲" + (row.optimalIndex - row.scoreIndex);
                        }
                        if (row.optimalIndex < row.scoreIndex) {
                            return "▼" + (row.scoreIndex - row.optimalIndex);
                        }
                        return "";
                    },
                    conditionalCellStyles: [
                        {
                            when: (row) => row.optimalIndex < row.scoreIndex,
                            style: {
                                color: "red",
                            },
                        },
                        {
                            when: (row) => row.optimalIndex > row.scoreIndex,
                            style: {
                                color: "green",
                            },
                        },
                    ],
                },
                {
                    name: "Naam",
                    selector: (row: MissedPointsTotalRow) => row.username,
                    sortable: true,
                },
                {
                    name: "Optimaal",
                    selector: (row: MissedPointsTotalRow) => row.optimaal,
                    sortable: true,
                },
            ]} data={optimalScoreRanking} />
        </div>
    );
};

export default RaceWrapMissedPoints;

function getMissedPointsTotal(missedPoints?: MissedPointsTableData[]) {
    const pointTotals = missedPoints?.map((userData) => {
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
        }).sort((a, b) => a.behaald - b.behaald) ?? []
    const optimalOrder = pointTotals.slice().sort((a: any, b: any) => a.optimaal - b.optimaal);
    return pointTotals.map((total, idx) => {
        return {
            ...total,
            scoreIndex: idx,
            optimalIndex: optimalOrder.findIndex((score: any) => score.username === total.username)
        }
    });
}
