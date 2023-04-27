import axios from "axios";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Card from "../components/card/Card";
import Button from "../components/ui/button/Button";

const Login = () => {
    const navigate = useNavigate()
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const submitForm = (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        axios.post('api/authentication/login', { email, password })
            .then(res => {
                if (res.status === 200) {
                    localStorage.setItem('authToken', res.data);
                    navigate('/')
                }
            })
            .catch((error) => {
                throw error // foute login geeft momenteel error? statuscode 400
            });
    }

    return (
        <Card>
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
                <Button
                    buttonClass=""
                    label="Log in"
                    type="submit"
                />
            </form>
        </Card>
    );
}

export default Login