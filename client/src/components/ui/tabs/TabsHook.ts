import { useState } from "react";

export type TabHook = ReturnType<typeof useTabs>;

export function useTabs(
  tabs: string[],
  initialTab: string,
  disabledTabs: string[]
) {
  if (!tabs.includes(initialTab)) {
    throw new Error("Tab not found");
  }
  const [selectedTab, setSelectedTab] = useState<string>(initialTab);

  return {
    tabs,
    selectedTab,
    setSelectedTab,
    disabledTabs,
  };
}
