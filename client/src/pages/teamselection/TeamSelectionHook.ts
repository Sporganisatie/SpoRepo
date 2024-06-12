import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import axios from "axios";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import { useRaceContext } from "../../components/shared/RaceContextProvider";
import { TeamSelectionData } from "./Models/TeamSelectionData";
import { SelectableEnum } from "../../models/SelectableEnum";

export function useTeamSelection() {
    const budgetParticipation = useBudgetContext();
    const raceId = useRaceContext();

    const queryKey = ["teamSelection", raceId, budgetParticipation] as const;
    const queryClient = useQueryClient();
    const { data, isLoading } = useQuery({
        queryKey,
        queryFn: ({ queryKey }) => fetchData(queryKey[1], queryKey[2]),
        throwOnError: true,
        staleTime: 3_600_000,
        gcTime: 3_600_000,
    });

    async function fetchData(raceId: number, budgetParticipation: boolean) {
        try {
            const { data } = await axios.get(
                `/api/TeamSelection?raceId=${raceId}&budgetParticipation=${budgetParticipation}`,
                {
                    params: {
                        raceId: raceId,
                        budgetParticipation: budgetParticipation,
                    },
                }
            );
            return data;
        } catch (error) {
            throw error;
        }
    }

    const addRiderMutation = useMutation(
        {
            mutationFn: addRider,
            onMutate: async (newRiderParticipationId) => {
                await queryClient.cancelQueries({ queryKey });
                const previousSelection = queryClient.getQueryData(queryKey);
                queryClient.setQueryData<TeamSelectionData>(
                    queryKey,
                    (oldData?: TeamSelectionData) => {
                        if (!oldData) {
                            return undefined;
                        }
                        handleMutation(oldData, newRiderParticipationId, true);
                    }
                );
                return { previousSelection };
            },
            onError: (err, newRiderParticipationId, context) => {
                queryClient.setQueryData(queryKey, context?.previousSelection);
            },
            onSettled: () => {
                queryClient.invalidateQueries({ queryKey });
            },
        },
        queryClient
    );

    async function addRider(riderParticipationId: number) {
        await axios.post(
            `/api/TeamSelection?riderParticipationId=${riderParticipationId}&raceId=${raceId}&budgetParticipation=${budgetParticipation}`,
            {
                params: {
                    riderParticipationId,
                    raceId: raceId,
                    budgetParticipation: budgetParticipation,
                },
            }
        );
        queryClient.invalidateQueries({
            queryKey: ["teamSelection", raceId, budgetParticipation],
        });
    }

    const removeRiderMutation = useMutation(
        {
            mutationFn: removeRider,
            onMutate: async (newRiderParticipationId) => {
                await queryClient.cancelQueries({ queryKey });
                const previousSelection = queryClient.getQueryData(queryKey);
                queryClient.setQueryData<TeamSelectionData>(
                    queryKey,
                    (oldData?: TeamSelectionData) => {
                        if (!oldData) {
                            return undefined;
                        }
                        handleMutation(oldData, newRiderParticipationId, false);
                    }
                );
                return { previousSelection };
            },
            onError: (err, newRiderParticipationId, context) => {
                queryClient.setQueryData(queryKey, context?.previousSelection);
            },
            onSettled: () => {
                queryClient.invalidateQueries({ queryKey });
            },
        },
        queryClient
    );

    async function removeRider(riderParticipationId: number) {
        await axios.delete(
            `/api/Teamselection?riderParticipationId=${riderParticipationId}&raceId=${raceId}&budgetParticipation=${budgetParticipation}`,
            {
                params: {
                    riderParticipationId,
                    raceId: raceId,
                    budgetParticipation: budgetParticipation,
                },
            }
        );
        queryClient.invalidateQueries({
            queryKey: ["stageSelection", raceId, budgetParticipation],
        });
    }

    function handleMutation(
        oldData: TeamSelectionData,
        riderParticipationId: number,
        selected: boolean
    ) {
        const rider = oldData.allRiders.find(
            ({ details }) =>
                details.riderParticipationId === riderParticipationId
        );
        if (rider === undefined) {
            throw new Error("Rider participation ID not found.");
        }
        if (selected) {
            oldData.team.push(rider.details);
            oldData.budgetOver -= rider.details.price;
            rider.selectable = SelectableEnum.Selected;
        } else {
            oldData.team = oldData.team.filter(
                (rider) => rider.riderParticipationId !== riderParticipationId
            );
            oldData.budgetOver += rider.details.price;
            rider.selectable = SelectableEnum.Open;
        }
        const forbiddenTeams = oldData.team
            .filter(
                (rider) =>
                    oldData.team.filter(
                        (teamRider) => teamRider.team === rider.team
                    ).length > 3
            )
            .map((rider) => rider.team);
        const teamSize = oldData.team.filter(
            (rider) => rider.riderParticipationId !== 0
        ).length;
        oldData.allRiders.forEach((r) => {
            if (
                r.selectable === SelectableEnum.Selected ||
                r.details.riderParticipationId ===
                    rider.details.riderParticipationId
            ) {
                return;
            }
            if (teamSize >= 20) {
                return (r.selectable = SelectableEnum.Max20);
            }
            if (
                r.details.price >
                oldData.budgetOver - (19 - teamSize) * 500_000
            ) {
                return (r.selectable = SelectableEnum.TooExpensive);
            }
            if (forbiddenTeams.includes(r.details.team)) {
                return (r.selectable = SelectableEnum.FourFromSameTeam);
            }
            return (r.selectable = SelectableEnum.Open);
        });
        sortTeam(oldData);
    }

    function sortTeam(data: TeamSelectionData) {
        data.team
            .sort((a, b) => b.riderParticipationId - a.riderParticipationId)
            .sort((a, b) => {
                if (a.type === b.type) {
                    return 0;
                }
                return a.type < b.type ? -1 : 1;
            })
            .sort((a, b) => b.price - a.price);
    }

    return {
        data,
        isLoading,
        addRider: addRiderMutation.mutate,
        removeRider: removeRiderMutation.mutate,
    };
}
export type StageSelectionHook = ReturnType<typeof useTeamSelection>;
