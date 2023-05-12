import SimpleClassification from './SimpleClassification';
import { Classifications } from '../models/StageSelectionData';

const ClassificationOverview = ({ data }: { data: Classifications }) => {
    return (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '10px' }}>
            <SimpleClassification rows={data.gc} title="Algemeen" resultColName="Tijd" />
            <SimpleClassification rows={data.points} title="Punten" resultColName="Punten" />
            <SimpleClassification rows={data.kom} title="Berg" resultColName="Punten" />
            <SimpleClassification rows={data.youth} title="Jongeren" resultColName="Tijd" />
        </div>
    );
};

export default ClassificationOverview;
