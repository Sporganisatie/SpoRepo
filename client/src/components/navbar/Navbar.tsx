import { Link } from "react-router-dom";
import "./navbar.css";
import StatsDropdown from "./Dropdowns/StatistiekenDropdown";
import ChartsDropdown from "./Dropdowns/ChartsDropdown";
// import SettingsDropdown from './Dropdowns/Settings'
// import MobileDropdown from './Dropdowns/Mobile'
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faLaptop, faShieldAlt } from "@fortawesome/free-solid-svg-icons";
import {
  useBudgetContext,
  useBudgetDispatch,
} from "../shared/BudgetContextProvider";
import { AuthToken } from "../../models/AuthToken";
import jwt_decode from "jwt-decode";
import Switch from "../ui/switch/Switch";
import { useRaceContext } from "../shared/RaceContextProvider";
// import { SRELogo } from '../shared/svg/all-icons.js'
// import FabFourSwitchButton from './fabFourSwitchButton';
// import jwt_decode from "jwt-decode";
// import { AuthToken } from '../../models/AuthToken';

const Navbar = () => {
  // TODO make dynamic values
  const isLoading = false;

  const race = useRaceContext();
  const budget = useBudgetContext();
  const dispatch = useBudgetDispatch();
  const isAdmin =
    localStorage.getItem("authToken") &&
    jwt_decode<AuthToken>(localStorage.getItem("authToken") ?? "").admin ===
    true;
  return (
    <div>
      <div className="navbar">
        {
          //<Link className='flex flex-grow md:flex-grow-0 h-full pt-2 pb-2 px-2 overflow-hidden' to='/'>
          //<SRELogo className={'h-full fill-current text-' + raceColor + " duration-300 hover:text-" + raceColorLight} />
          //</Link>
        }
        {race > 0 && (
          <Link className="navbar_link" to={"/"}>
            <span>Current stage</span>
          </Link>
        )}
        {!isLoading && <ChartsDropdown raceSelected={race > 0} />}
        {!isLoading && <StatsDropdown raceSelected={race > 0} />}
        <Link className="navbar_link" to="/regelspunten">
          <span>Regels/Punten</span>
        </Link>
        {/* {isAdmin && (
          <Link className="navbar_link" to="/admin">
            <FontAwesomeIcon icon={faChartSimple} />
          </Link>
        )} */}
        {isAdmin && (
          <Link className="navbar_link" to="/admin">
            <FontAwesomeIcon icon={faShieldAlt} />
          </Link>
        )}
        {isAdmin && (
          <Link className="navbar_link" to="/designsandbox">
            <FontAwesomeIcon icon={faLaptop} />
          </Link>
        )}
        {/* <Link className="navbar_link" to="/profile">
          <FontAwesomeIcon icon={faUser} />
        </Link> */}
        {localStorage.getItem("authToken") &&
          jwt_decode<AuthToken>(localStorage.getItem("authToken") ?? "").id <=
          5 && (
            <Switch
              value={budget}
              handleOnChange={() => dispatch({})}
              sliderContent="€"
            />
          )}
      </div>
    </div>
  );
};

export default Navbar;
