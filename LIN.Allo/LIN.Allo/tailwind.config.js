/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["../**/*{html,razor,js,cs}"],
    theme: {
        extend: {
            width: {
                'one-third': '33.333333%',
            }
        },
    },
    plugins: [],
}

