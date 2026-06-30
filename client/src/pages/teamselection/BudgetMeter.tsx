interface BudgetMeterProps {
  used: number;
  total: number;
  openSpaces: number;
}

function formatM(value: number): string {
  return `${(value / 1_000_000).toFixed(2)}M`;
}

const BudgetMeter = ({ used, total, openSpaces }: BudgetMeterProps) => {
  const remaining = total - used;
  const over = remaining < 0;
  const textClass = over ? "over" : "";
  const remainingPerOpenSpace = openSpaces > 0 ? remaining / openSpaces : 0;

  return (
    <div className="budget-meter">
      <span className={`tabular ${textClass}`}>Plekken: {openSpaces}</span>
      <span className={`tabular ${textClass}`}>Budget: {formatM(remaining)}</span>
      <span className={`tabular ${textClass}`}>Per plek: {formatM(remainingPerOpenSpace)}</span>
    </div>
  );
};

export default BudgetMeter;
