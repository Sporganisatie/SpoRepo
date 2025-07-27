import { useEffect, useState } from "react";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { useRaceContext } from "../../components/shared/RaceContextProvider";
import axios from "axios";
import SreDataTable from "../../components/shared/SreDataTable";
import RaceWrapAward from "./RaceWrapAward";

interface UserRank {
    username: string;
    ranks: number[];
}

interface RankedUserRank {
    username: string;
    wins: number;
    rank: number;
}

const RaceWrapStages = () => {
    const raceId = useRaceContext();
    const budgetParticipation = useBudgetContext();

    const [data, setData] = useState<RankedUserRank[]>([]);

    useEffect(() => {
        axios
            .get(`/api/Statistics/etappeUitslagen`, {
                params: { raceId, budgetParticipation },
            })
            .then((res) => {
                const rankedRanks: RankedUserRank[] = [];
                let prevScore: number = 99;
                let rank = 0;
                res.data.userRanks.forEach((userRank: UserRank) => {
                    const wins = userRank.ranks[0];
                    if (prevScore !== wins) {
                        rank++;
                    }
                    rankedRanks.push({
                        username: userRank.username,
                        wins,
                        rank,
                    });
                    prevScore = wins;
                });
                setData(rankedRanks);
            })
            .catch((error) => {});
    }, [raceId, budgetParticipation]);

    return (
        <div>
            {data ? (
                <RaceWrapAward
                    awardName={"🏴‍☠️ Rittenkaper"}
                    awardWinners={data.filter((u) => u.rank === 1).map((u) => u.username).join(" & ")}
                />
            ) : (
                <></>
            )}
            <SreDataTable
                data={data}
                title={"Etappewinsten"}
                columns={[
                    {
                        name: "",
                        selector: (row: RankedUserRank) => row.rank,
                        sortable: true,
                    },
                    {
                        name: "Naam",
                        selector: (row: RankedUserRank) => row.username,
                        sortable: true,
                    },
                    {
                        name: "Overwinningen",
                        selector: (row: RankedUserRank) => row.wins,
                        sortable: true,
                    },
                ]}
            />
        </div>
    );
};

export default RaceWrapStages;
