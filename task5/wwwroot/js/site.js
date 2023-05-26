let ps = [...document.getElementsByTagName('li')].map(x => x.innerText);

let finalString = ps.join('\r\n');



const fileHandle = await window.showSaveFilePicker();
const fileStream = await fileHandle.createWritable();
await fileStream.write(new Blob([...finalString], { type: "text/plain" }));
await fileStream.close();