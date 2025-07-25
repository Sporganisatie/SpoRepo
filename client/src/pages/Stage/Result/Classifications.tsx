import { useEffect, useState } from 'react';
import ClassificationTable from '../Selection/ClassificationTable';
import { Classifications } from '../models/StageSelectionData';
import './StageClassifications.css';

const StageClassifications = ({ data, finalStandings }: { data: Classifications, finalStandings: boolean }) => {
    const [activeButton, setActiveButton] = useState('Etappe');

    useEffect(() => {
        setActiveButton(finalStandings ? 'Algemeen' : 'Etappe');
    }, [finalStandings]);

    const handleClick = (title: string) => {
        setActiveButton(title);
    };

    return (
        <div>
            <div>
                {!finalStandings && (
                    <button
                        className={
                            (activeButton === 'Etappe' ? 'active' : '')
                            + ((data.stage?.length ?? 0) > 0 ? '' : 'disabled')}
                        disabled={data.stage?.length === 0}
                        onClick={() => handleClick('Etappe')}
                    >
                        Etappe
                    </button>
                )}
                <button
                    className={
                        (activeButton === 'Algemeen' ? 'active' : '')
                        + ((data.gc?.length ?? 0) > 0 ? '' : 'disabled')}
                    disabled={data.gc?.length === 0}
                    onClick={() => handleClick('Algemeen')}
                >
                    Algemeen
                </button>
                <button
                    className={
                        (activeButton === 'Punten' ? 'active' : '')
                        + ((data.points?.length ?? 0) > 0 ? '' : 'disabled')}
                    disabled={data.points?.length === 0}
                    onClick={() => handleClick('Punten')}
                >
                    Punten
                </button>
                <button
                    className={
                        (activeButton === 'Berg' ? 'active' : '')
                        + ((data.kom?.length ?? 0) > 0 ? '' : 'disabled')}
                    disabled={data.kom?.length === 0}
                    onClick={() => handleClick('Berg')}
                >
                    Berg
                </button>
                <button
                    className={
                        (activeButton === 'Jongeren' ? 'active' : '')
                        + ((data.youth?.length ?? 0) > 0 ? '' : 'disabled')}
                    disabled={data.youth?.length === 0}
                    onClick={() => handleClick('Jongeren')}
                >
                    Jongeren
                </button>
            </div>
            {activeButton === 'Etappe' && (
                <ClassificationTable rows={data.stage ?? []} title="Etappe" resultColName="Tijd" pagination={true} />
            )}
            {activeButton === 'Algemeen' && (
                <ClassificationTable rows={data.gc} title="Algemeen" resultColName="Tijd" pagination={true} showRankChange={true} />
            )}
            {activeButton === 'Punten' && (
                <ClassificationTable rows={data.points} title="Punten" resultColName="Punten" pagination={true} showRankChange={true} />
            )}
            {activeButton === 'Berg' && (
                <ClassificationTable rows={data.kom} title="Berg" resultColName="Punten" pagination={true} showRankChange={true} />
            )}
            {activeButton === 'Jongeren' && (
                <ClassificationTable rows={data.youth} title="Jongeren" resultColName="Tijd" pagination={true} showRankChange={true} />
            )}
        </div>
    );
};

export default StageClassifications;
