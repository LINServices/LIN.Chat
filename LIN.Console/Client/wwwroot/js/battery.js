

async function getLevel(e) {

    console.log(e);
    const battery = await navigator.getBattery();
    let level = battery.level * 100;
    console.log(level);
    return level;
}


async function getChargin(e) {
    console.log(e);
    const battery = await navigator.getBattery();
    let chargin = battery.charging;
    console.log(chargin);
    return chargin;
}
