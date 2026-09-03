import "./StageScoreSpreadChart.css";
import { colors } from "../../Charts/ChartsHelper";

interface UserStageScores {
  username: string;
  stageScores: number[];
}

interface StageScoreSpreadChartProps {
  data: UserStageScores[];
}

const GUIDE_LINES = [0, 100, 200, 300];
const MAX_SCORE = 400;

const clampScore = (score: number) => Math.max(0, Math.min(MAX_SCORE, score));
const getUserColor = (index: number) => colors[index % colors.length] ?? "#38bdf8";

const StageScoreSpreadChart = ({ data }: StageScoreSpreadChartProps) => (
  <div className="panel score-spread-panel">
    <div className="score-spread-header">
      <h2 className="score-spread-title">Score Spreiding</h2>
    </div>
    <div className="score-spread-body">
      <div className="score-spread-axis-row">
        <div className="score-spread-name-spacer" />
        <div className="score-spread-axis">
          {GUIDE_LINES.map((line) => (
            <span
              key={line}
              className="score-spread-axis-label"
              style={{ left: `${(line / MAX_SCORE) * 100}%` }}
            >
              {line}
            </span>
          ))}
          <span className="score-spread-axis-label score-spread-axis-label-right">{MAX_SCORE}</span>
        </div>
      </div>

      {data.map((user, userIndex) => {
        const userColor = getUserColor(userIndex);
        return (
        <div key={user.username} className="score-spread-row">
          <div className="score-spread-user">{user.username}</div>
          <div className="score-spread-track">
            {GUIDE_LINES.map((line) => (
              <span
                key={line}
                className="score-spread-guide"
                style={{ left: `${(line / MAX_SCORE) * 100}%` }}
              />
            ))}
            {user.stageScores.map((score, index) => (
              <span
                key={`${user.username}-${index}`}
                className="score-spread-dot"
                title={`Etappe ${index + 1}: ${score}`}
                style={{
                  left: `${(clampScore(score) / MAX_SCORE) * 100}%`,
                  top: `${50 + ((index % 3) - 1) * 14}%`,
                  backgroundColor: userColor,
                }}
              />
            ))}
          </div>
        </div>
        );
      })}
    </div>
  </div>
);

export default StageScoreSpreadChart;
