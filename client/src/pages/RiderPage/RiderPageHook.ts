import axios from "axios";
import { useQuery } from "@tanstack/react-query";

export function useRiderPage(riderId?: string) {
  if (riderId === undefined) {
    throw new Error("Expected riderId");
  }

  const { data: riderInfo } = useQuery({
    queryKey: ["stage", riderId],
    queryFn: () => fetchRiderInfo(riderId),
    staleTime: 10000,
  });

  function fetchRiderInfo(riderId?: string): Promise<any> {
    console.log(riderId)
    if (riderId === undefined) {
      throw new Error("Expected riderId");
    }
    return axios
      .get(`/api/rider`, { params: { riderId } })
      .then((res) => {
        return res.data;
      })
      .catch(function (error) {
        throw error;
      });
  }

  return riderInfo;
}
