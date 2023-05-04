// import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import StageSelection from "./Selection/StageSelection";

enum StageStateEnum {
    None,
    Selection,
    Started
}


const Stage = () => {
    let navigate = useNavigate();
    let { raceId, stagenr } = useParams();
    const [stageState, setStageState] = useState(StageStateEnum.None)

    useEffect(() => {
        // axios.get(`/api/stage`, { params: { raceId, stagenr } })
        //     .then(res => {
        //         setStageState(res.data)
        //     })
        //     .catch(function (error) {
        //         throw error
        //     });
        setStageState(StageStateEnum.Selection)
    }, [raceId, navigate])


    // Wss dit nog omgooien zodat de etappe navigatie hierin gezet kan worden
    return ((() => {
        switch (stageState) {
            case StageStateEnum.Selection:
                return <StageSelection raceId={raceId!} stagenr={stagenr!} />
            // case StageStateEnum.Started:
            //     return <StageResult />
            default:
                return <></>
        }
    })())
}

export default Stage;