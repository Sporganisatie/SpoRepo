// import axios from "axios";
import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import StageResult from "./Result/StageResult";
import StageSelection from "./Selection/StageSelection";
import ArrowSelect from "../../components/ArrowSelect";
import { SelectOption } from "../../components/Select";

enum StageStateEnum {
    None,
    Selection,
    Started
}

const stages: SelectOption<string>[] = Array.from({ length: 21 }, (_, i) => ({
    displayValue: (i + 1).toString(),
    value: (i + 1).toString(),
}));

const Stage = () => {
    let navigate = useNavigate();
    let { raceId, stagenr } = useParams();
    const [stageState, setStageState] = useState(StageStateEnum.None)

    useEffect(() => {
        axios.get(`/api/stage`, { params: { raceId, stagenr } })
            .then(res => {
                setStageState(res.data)
            })
            .catch(function (error) {
                throw error
            });
    }, [raceId, stagenr, navigate])

    const navigateStage = (newStage: string) => {
        if (parseInt(newStage) < parseInt(stagenr!) && stageState === StageStateEnum.Selection) setStageState(StageStateEnum.None);
        navigate(`/stage/${raceId}/${newStage}`)
    }

    return (
        <div>
            <ArrowSelect
                value={stagenr}
                allowLooping={false}
                options={stages}
                onChange={(selectedValue) => { navigateStage(selectedValue) }} />
            {stageState === StageStateEnum.Selection && <StageSelection raceId={raceId!} stagenr={stagenr!} />}
            {stageState === StageStateEnum.Started && <StageResult raceId={raceId!} stagenr={stagenr!} />}
        </div>
    )
}

export default Stage;