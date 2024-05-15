import { TabHook } from "./TabsHook";
import "./tabs.css";

const Tabs = ({
  tabHook: { tabs, selectedTab, setSelectedTab, disabledTabs },
}: {
  tabHook: TabHook;
}) => {
  const tabDivs = tabs.map((tab) => {
    return (
      <div
        className={
          "tab " +
          (selectedTab === tab ? "selected " : "") +
          (disabledTabs.includes(tab) ? "disabled " : "")
        }
        onClick={
          disabledTabs.includes(tab) ? () => {} : () => setSelectedTab(tab)
        }
      >
        {tab}
      </div>
    );
  });

  return <div className="tab-container">{tabDivs}</div>;
};

export default Tabs;
