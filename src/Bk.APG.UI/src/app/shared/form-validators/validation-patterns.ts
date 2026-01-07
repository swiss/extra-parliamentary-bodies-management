/* eslint-disable */
export const ECH_EMAIL_PATTERN =
    /^(?!.{100})[A-Za-zäöüÄÖÜàáâãåæçèéêëìíîïðñòóôõøùúûýþÿ0-9!#-'\*\+\-/=\?\^_`\{-~]+(\.[A-Za-zäöüÄÖÜàáâãåæçèéêëìíîïðñòóôõøùúûýþÿ0-9!#-'\*\+\-/=\?\^_`\{-~]+)*@[A-Za-zäöüÄÖÜàáâãåæçèéêëìíîïðñòóôõøùúûýþÿ0-9!#-'\*\+\-/=\?\^_`\{-~]+(\.[A-Za-zäöüÄÖÜàáâãåæçèéêëìíîïðñòóôõøùúûýþÿ0-9!#-'\*\+\-/=\?\^_`\{-~]+)+$/;
/* eslint-enable */

// This is adopted from the ech pattern (/\t{10,20}/)
// to enforce the international format(requiring to start with +)
// and to increase the usability (by allowing spaces)
// Inspired by https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s03.html
// Archived: https://web.archive.org/web/20210622145010/https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s03.html
export const TEL_PATTERN = /^\+(?:[0-9] ?){6,19}[0-9]$/;

// eslint-disable-next-line
export const URL_PATTERN = /^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$/;
