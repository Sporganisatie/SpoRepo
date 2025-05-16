import { RouterProvider, useParams } from "react-router-dom";
import Layout from "./components/Layout";
import "./index.css";
import { useEffect, useState } from "react";
import { setupAxiosInterceptor } from "./AxiosInterceptor";
import { useNavigate } from "react-router-dom";
import router from "./Pages";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { RaceStateProvider, useRaceDispatch } from "./components/shared/RaceContextProvider";
import { overrideDarkMode } from "./components/ui/table/themes";

const queryClient = new QueryClient();

export default function App() {
    return (
        <RaceStateProvider>
            <QueryClientProvider client={queryClient}>
                <RouterProvider router={router} />
            </QueryClientProvider>
        </RaceStateProvider>
    );
}

export function Root() {
    const { raceId } = useParams();
    const raceDispatch = useRaceDispatch();
    overrideDarkMode();

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
