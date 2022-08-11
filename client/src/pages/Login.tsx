// import api from "../api";

import axios from "axios";
import { useState } from "react";

const Login = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const submitForm = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        axios.post('authentication/login', { email, password })
            .then(res => {
                if (res.status === 200) {
                    console.log(res.data);
                    localStorage.setItem('authToken', res.data);
                } else {
                }
            })
            .catch(function (error) {
                throw error // foute login geeft momenteel error? statuscode 400
            });
    }

    return (
        <div className="flex flex-col max-w-full m-auto md:mr-12 md:ml-4 mt-12 bg-white p-5 shadow-2xl rounded-md">
            <div className="mt-1 mb-6 font-bold text-gray-600 text-center">
                'Log in to manage your team'
            </div>
            <form onSubmit={submitForm}>
                <div className="ml-1 mb-1 text-sm text-gray-600">E-mail address</div>
                <div className="mb-4">
                    <input
                        className="form-control"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        type="email"
                        placeholder="Email Address" />
                </div>
                <div className="ml-1 mb-1 text-sm text-gray-600">Password</div>
                <div className="mb-4">
                    <input
                        className="form-control"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        type="password"
                        placeholder="Password" />
                </div>
                <button className="landing_button mt-4 float-right rounded-md shadow-md">
                    'Log in'
                </button>
            </form>
            <div className="float-left">
                {/* <PasswordRecoveryModal /> */} Todo import PasswordRecoveryModal
            </div>
        </div>
    );
}

export default Login