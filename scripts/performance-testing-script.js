import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from "k6/metrics";


var failureRate = new Rate("check_failure_rate");

export const options = {
  vus: 5, //default value, can be overwritten by passing the --vus CLI option
  duration: '10s', //default value, can be overwritten by passing the --duration CLI option
  thresholds: {
    // We want the 95th percentile of all HTTP request durations to be less than 500ms
    "http_req_duration": ["p(95)<1000"],
    // Thresholds based on the custom metric we defined and use to track application failures
    "check_failure_rate": [
        // Global failure rate should be less than 1%
        "rate<0.01",
        // Abort the test early if it climbs over 5%
        { threshold: "rate<=0.05", abortOnFail: true },
    ],
  }
};
export const params = {
    headers: { 'Authorization': `${__ENV.TOKEN}`},
};
export default function () {
  var apiUrl = `${__ENV.API_URL}`
  if (!apiUrl.includes('https://'))
  {
    apiUrl = "https://" + apiUrl;
  }
  const res = http.get(apiUrl, params);

  let checkRes = check(res, { 'status was 200': (r) => r.status == 200 });

  failureRate.add(!checkRes);

  sleep(1);
}