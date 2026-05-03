import { QueryClient } from "@tanstack/react-query";
import { ONE_HOUR_MS } from "../lib/constants";

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: ONE_HOUR_MS,
      gcTime: ONE_HOUR_MS,
    },
  },
});
