import { useEffect, useState } from "react";
import axios from "../../api/client";
import "./StageProfiles.css";

type StageProfilesData = {
    baseUrl: string;
    profile: string;
    climbs: string[];
    finishProfile: string;
};

const emptyProfiles: StageProfilesData = {
    baseUrl: "",
    profile: "",
    climbs: [],
    finishProfile: "",
};

const StageProfiles = ({
    raceId,
    stageNr,
}: {
    raceId: number;
    stageNr: string;
}) => {
    const [data, setData] = useState<StageProfilesData>(emptyProfiles);
    const [loading, setLoading] = useState<boolean>(true);

    useEffect(() => {
        let cancelled = false;

        const loadProfiles = async () => {
            setLoading(true);
            try {
                const response = await axios.get(`/api/Stage/profiles`, { params: { raceId, stageNr } });
                if (!cancelled) {
                    setData(response.data as StageProfilesData);
                }
            } catch (err) {
                console.error(err);
                if (!cancelled) {
                    setData(emptyProfiles);
                }
            } finally {
                if (!cancelled) {
                    setLoading(false);
                }
            }
        };

        loadProfiles();

        return () => {
            cancelled = true;
        };
    }, [raceId, stageNr]);

    if (loading) {
        return <div className="stage-profiles-status">Profielen laden...</div>;
    }

    const hasProfiles = Boolean(data.profile || data.finishProfile || data.climbs.length > 0);
    const toImageUrl = (fileName: string) => (fileName ? `${data.baseUrl}/${fileName}` : "");

    if (!hasProfiles) {
        return <div className="stage-profiles-status">Geen profielen beschikbaar voor deze etappe.</div>;
    }

    return (
        <div className="stage-profiles">
            {data.profile && (
                <div className="stage-profiles-item">
                    <h3>Profiel</h3>
                    <img src={toImageUrl(data.profile)} alt="Etappeprofiel" loading="lazy" />
                </div>
            )}

            {data.climbs.length > 0 && (
                <div className="stage-profiles-item">
                    <h3>Beklimmingen</h3>
                    <div className="stage-profiles-climbs">
                        {data.climbs.map((climbUrl, index) => (
                            <img
                                key={`${climbUrl}-${index}`}
                                src={toImageUrl(climbUrl)}
                                alt={`Beklimmingsprofiel ${index + 1}`}
                                loading="lazy"
                            />
                        ))}
                    </div>
                </div>
            )}

            {data.finishProfile && (
                <div className="stage-profiles-item">
                    <h3>Finishprofiel</h3>
                    <img src={toImageUrl(data.finishProfile)} alt="Finishprofiel" loading="lazy" />
                </div>
            )}
        </div>
    );
};

export default StageProfiles;
