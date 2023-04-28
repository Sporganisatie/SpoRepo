import { Link } from 'react-router-dom';
import './navbar.css';
import StatsDropdown from './Dropdowns/StatistiekenDropdown'
import ChartsDropdown from './Dropdowns/ChartsDropdown';
// import SettingsDropdown from './Dropdowns/Settings'
// import MobileDropdown from './Dropdowns/Mobile'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faLaptop, faShieldAlt, faUser } from "@fortawesome/free-solid-svg-icons";
import { useBudgetContext, useBudgetDispatch } from '../shared/BudgetContextProvider';
import { AuthToken } from '../../models/AuthToken';
import jwt_decode from "jwt-decode";
// import { SRELogo } from '../shared/svg/all-icons.js'
// import BudgetSwitchButton from './budgetSwitchButton';
// import FabFourSwitchButton from './fabFourSwitchButton';
// import jwt_decode from "jwt-decode";
// import { AuthToken } from '../../models/AuthToken';

interface NavbarProps {
  currentStageLink: string,
  isAdmin: boolean,
  isLoading: boolean,
  isLoggedIn: boolean,
  racename: string,
}

const Navbar = (props: NavbarProps) => {
  const race = props.racename;
  const budget = useBudgetContext();
  const dispatch = useBudgetDispatch();

  return (
    <div className="navbar">
      {
        //<Link className='flex flex-grow md:flex-grow-0 h-full pt-2 pb-2 px-2 overflow-hidden' to='/'>
        //<SRELogo className={'h-full fill-current text-' + raceColor + " duration-300 hover:text-" + raceColorLight} />
        //</Link>
      }
      {race !== null && <Link className='navbar_link' to={props.currentStageLink}><span>Current stage</span></Link>}
      {!props.isLoading && <ChartsDropdown raceSelected={race !== null} />}
      {!props.isLoading && <StatsDropdown raceSelected={race !== null} />}
      {props.isAdmin &&
        <Link className='navbar_link' to='/admin-sqlinterface'>
          <FontAwesomeIcon icon={faShieldAlt} />
        </Link>
      }
      <Link className='navbar_link' to='/designsandbox'>
        <FontAwesomeIcon icon={faLaptop} />
      </Link>
      <Link className='navbar_link' to='/profile'>
        <FontAwesomeIcon icon={faUser} />
      </Link>
      {localStorage.getItem('authToken') ? (jwt_decode<AuthToken>(localStorage.getItem('authToken') ?? "")).id <= 5 &&
        <div> Budget
          <input type="checkbox" checked={budget} onClick={() => dispatch({})} onChange={() => { }} /></div> : <></>}
    </div>
  )
}

export default Navbar
