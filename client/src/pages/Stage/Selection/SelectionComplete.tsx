import "./SelectionComplete.css";

const SelectionsComplete = ({ compleet, budgetCompleet }: { compleet: number, budgetCompleet: number | null }) => {
    return (
        <div>
            {
                budgetCompleet != null ?
                    <div className={"completeContainer " + ((compleet + budgetCompleet) === 20 ? "allCompleet" : "")}>
                        Compleet:
                        <SelectionsCompleteRow compleet={compleet} className="gewoonCompleet" rowText="Gewoon" />
                        <SelectionsCompleteRow compleet={budgetCompleet} className="budgetCompleet" rowText="Budget" />
                    </div>
                    :
                    <div className={"completeContainerNonBudget " + ((compleet) === 10 ? "allCompleet" : "")}>
                        Compleet:
                        <SelectionsCompleteRow compleet={compleet} className="gewoonCompleet" rowText="Team" />
                    </div>
            }
        </div>
    );
};

const SelectionsCompleteRow = ({ compleet, className, rowText }: { compleet: number, className: string, rowText: string }) => {
    return (
        <div className={className}>
            <div style={{ width: compleet * 10 + "%" }} className={"backgroundCompleet"}></div>
            <div className="textCompleet">{rowText} </div>
        </div>
    );
};

export default SelectionsComplete;
