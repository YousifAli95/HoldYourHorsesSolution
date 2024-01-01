//**** Variables ****/

// TIME_FACTOR is a property that let us control the overall speed of all horses.
// Higher value results in slower horses, lower value will increase the speed of the horses. 
const TIME_FACTOR = 8; 

//**** Functions ****//

function setCircleText() {
    // Get the circles where the horse finish position will be written, and sort these circles based on horse finishing position
    const circles = Array.from(document.querySelectorAll(".circle")).sort((a, b) => {
        // Calculate where each horse will end up in the race and if there will be any ties
        const dataA = a.parentElement.dataset;
        const dataB = b.parentElement.dataset;

        const deltaTime = parseFloat(dataA.time) - parseFloat(dataB.time);

        dataA.tie = deltaTime === 0 ? "true" : dataA.tie;

        return deltaTime;
    });

    let counter = 0;

    // Set the horse finishing position in the circle text
    circles.forEach((circle) => {
        const parentData = circle.parentElement.dataset;

        // Increment only if not a tie
        counter += parentData.tie === "true" ? 0 : 1;

        // Set the circle text to 1:a, 2:a, 3:a etc...
        circle.innerHTML = `${counter}:a`;
    });
}

function setGoalHeight() {
    // Set the height of the goal depending on how many horses there are in the race
    const goal = document.querySelector(".goal");
    const numberOfHorses = parseInt(document.querySelectorAll(".box").length);
    const GOAL_HEIGHT_PER_HORSE_PX = 150;

    goal.style.height = `${GOAL_HEIGHT_PER_HORSE_PX * numberOfHorses}px`;
}

function setRacingAnimation() {
    const horses = document.querySelectorAll(".horse");

    horses.forEach((horse) => {
        // Get horse properties from HTML code
        const { category, density, horsepowers, wood, country } = horse.dataset;

        // Get int representation of these properties
        const intCategory = getCategory(category);
        const intCountry = getCountry(country);

        // Calculate the time needed to finish the race for each horse
        const t = (intCategory * density * intCountry * TIME_FACTOR) / horsepowers;
        horse.dataset.time = t;

        // Set animation properties
        Array.from(horse.children).forEach((child) => {
            const style = child.style;
            style.animationTimingFunction = getBezierFromMaterial(wood);
            style.animationDuration = `${t}s`;
            style.animationFillMode = "forwards";
        });
    });
}

function getBezierFromMaterial(material) {
    const materialMap = {
        "Ek": "cubic-bezier(0.27, 0.56, 0, 1.02)",
        "Furu": "cubic-bezier(0.74, 0.22, 0.6, 0.86)",
        "Mahogony": "cubic-bezier(0.45, 0.45, 0.75, 0.75)",
        "Gran": "cubic-bezier(0.97, -1.01, 0, 0.12)"
    };

    return materialMap[material] || "cubic-bezier(0, 0.45, 0.51, 0.82)";
}

function getCategory(category) {
    const categoryMap = {
        "Sport": 0.7,
        "Fritid": 1,
        "Barn": 1.3
    };

    return categoryMap[category] || 1.1; // Default to 1.1 if kategori is not found
}

function getCountry(country) {
    const countryMap = {
        "Sverige": 0.7,
        "Norge": 1,
        "Danmark": 1.3
    };

    return countryMap[country] || 1.1; // Default to 1.1 if country is not found
}


//**** Code starts here **** //

setGoalHeight();
setRacingAnimation();
setCircleText();



