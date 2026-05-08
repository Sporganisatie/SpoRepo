import type { DropdownLinkProps } from "./DropdownLink";
import { DropdownLink } from "./DropdownLink";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import type { IconDefinition } from "@fortawesome/fontawesome-svg-core";
import { faAngleDown, faAngleUp } from "@fortawesome/free-solid-svg-icons";
import { useEffect, useState } from "react";

interface DropdownMenuProps {
  alwaysLinks: DropdownLinkProps[];
  raceOnlyLinks: DropdownLinkProps[];
  name: string;
  icon?: IconDefinition;
  raceSelected: boolean;
}

const DropdownMenu = (props: DropdownMenuProps) => {
  const [showMenu, setShowMenu] = useState(false);

  // Click-outside-to-close. Listener is only attached while open and torn
  // down on close/unmount. Deferred a tick so the opening click doesn't
  // immediately close the menu.
  useEffect(() => {
    if (!showMenu) return;
    const close = () => setShowMenu(false);
    const id = setTimeout(() => window.addEventListener("click", close), 0);
    return () => {
      clearTimeout(id);
      window.removeEventListener("click", close);
    };
  }, [showMenu]);

  const alwaysLinks = props.alwaysLinks.map((link) => (
    <DropdownLink key={link.title} url={link.url} title={link.title} />
  ));
  const raceOnlyLinks = props.raceOnlyLinks.map((link) => (
    <DropdownLink key={link.title} url={link.url} title={link.title} />
  ));

  return (
    <div className="navbar_link" title={props.icon ? props.name : undefined}>
      <span onClick={() => setShowMenu(true)}>
        {props.icon && <FontAwesomeIcon icon={props.icon} className="nav-on-mobile" />}
        <span className={props.icon ? "nav-on-desktop" : ""}>{props.name}</span>{" "}
        <FontAwesomeIcon icon={showMenu ? faAngleUp : faAngleDown} />
      </span>
      {showMenu ? (
        <div className="navbar_dropdown-content">
          {alwaysLinks}
          {props.raceSelected && raceOnlyLinks}
        </div>
      ) : null}
    </div>
  );
};

export default DropdownMenu;
