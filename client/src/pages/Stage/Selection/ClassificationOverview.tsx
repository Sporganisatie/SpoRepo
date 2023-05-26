import ClassificationTable from './ClassificationTable';
import { Classifications } from '../models/StageSelectionData';

const ClassificationOverview = ({ data }: { data: Classifications }) => {
    return (
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(2, 1fr)', gap: '10px' }}>
            <ClassificationTable rows={data.gc} title="Algemeen" resultColName="Tijd" />
            <ClassificationTable rows={data.points} title="Punten" resultColName="Punten" />
            <ClassificationTable rows={data.kom} title="Berg" resultColName="Punten" />
            <ClassificationTable rows={data.youth} title="Jongeren" resultColName="Tijd" />
        </div>
    );
};

export default ClassificationOverview;
