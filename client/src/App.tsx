import { RouterProvider, useParams } from "react-router-dom";
import Layout from "./components/Layout";
import "./index.css";
import { StrictMode, useEffect, useState } from "react";
import { setupAxiosInterceptor } from "./AxiosInterceptor";
import { useNavigate } from "react-router-dom";
import router from "./Pages";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { RaceStateProvider, useRaceDispatch } from "./components/shared/RaceContextProvider";

const queryClient = new QueryClient();

export default function App() {
  return (
    <StrictMode>
      <RaceStateProvider>
        <QueryClientProvider client={queryClient}>
          <RouterProvider router={router} />
        </QueryClientProvider>
      </RaceStateProvider>
    </StrictMode>
  );
}

export function Root() {
  const { raceId } = useParams();
  const raceDispatch = useRaceDispatch();

  useEffect(() => {
    if (raceId !== undefined) {
      raceDispatch(parseInt(raceId));
    }
  }, [raceId, raceDispatch]);

  const navigate = useNavigate();
  const [axiosInterceptorDone, setAxiosInterceptorDone] = useState(false);
  useEffect(() => {
    // onLoad
    setupAxiosInterceptor(navigate);
    setAxiosInterceptorDone(true);
  }, [navigate]);

  return axiosInterceptorDone ? <Layout /> : <></>;
}
