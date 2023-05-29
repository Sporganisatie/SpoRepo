import { EtappeUitslag } from "../Statistics/EtappeUitslagen/UitslagenTable";
export const colors = ["#00d60e", "#1C43FF", "#FF0000", "#F9F200", "#A900F9", "#FF8000", "#194D33", "#00DEF9", "#F900BB", "#6C3703"];

export const convertData = (data: EtappeUitslag[], invert: boolean = false): any => { // TODO deze logica naar BE
    const convertedData: any[] = [];
    for (let et = 0; et < data.length; et++) {
        const stageData: { [key: string]: any } = {
            name: data[et].name + " " + data[et].year
        };

        for (let i = 0; i < data[et].usernamesAndScores.length; i++) {
            const { username, score } = data[et].usernamesAndScores[i];
            stageData[username] = invert ? score * -1 : score;
        }
        convertedData.push(stageData)
    }
    for (let et = data.length; et < 8; et++) {
        convertedData.push({ name: et })
    }

    return convertedData;
};
