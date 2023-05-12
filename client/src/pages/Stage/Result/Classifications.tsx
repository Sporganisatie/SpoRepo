import { useState } from 'react';
import SimpleClassification from '../Selection/SimpleClassification';
import { Classifications } from '../models/StageSelectionData';
import './StageClassifications.css';

const StageClassifications = ({ data }: { data: Classifications }) => {
    const [activeButton, setActiveButton] = useState('Etappe');

    const handleClick = (title: string) => {
        setActiveButton(title);
    };

    return (
        <div>
            <div>
                <button
                    className={activeButton === 'Etappe' ? 'active' : ''}
                    onClick={() => handleClick('Etappe')}
                >
                    Etappe
                </button>
                <button
                    className={activeButton === 'Algemeen' ? 'active' : ''}
                    onClick={() => handleClick('Algemeen')}
                >
                    Algemeen
                </button>
                <button
                    className={activeButton === 'Punten' ? 'active' : ''}
                    onClick={() => handleClick('Punten')}
                >
                    Punten
                </button>
                <button
                    className={activeButton === 'Berg' ? 'active' : ''}
                    onClick={() => handleClick('Berg')}
                >
                    Berg
                </button>
                <button
                    className={activeButton === 'Jongeren' ? 'active' : ''}
                    onClick={() => handleClick('Jongeren')}
                >
                    Jongeren
                </button>
            </div>
            {activeButton === 'Etappe' && (
                <SimpleClassification rows={data.stage ?? []} title="Etappe" resultColName="Tijd" pagination={true} />
            )}
            {activeButton === 'Algemeen' && (
                <SimpleClassification rows={data.gc} title="Algemeen" resultColName="Tijd" pagination={true} />
            )}
            {activeButton === 'Punten' && (
                <SimpleClassification rows={data.points} title="Punten" resultColName="Punten" pagination={true} />
            )}
            {activeButton === 'Berg' && (
                <SimpleClassification rows={data.kom} title="Berg" resultColName="Punten" pagination={true} />
            )}
            {activeButton === 'Jongeren' && (
                <SimpleClassification rows={data.youth} title="Jongeren" resultColName="Tijd" pagination={true} />
            )}
        </div>
    );
};

export default StageClassifications;
