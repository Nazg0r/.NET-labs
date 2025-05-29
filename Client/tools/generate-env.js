import { writeFileSync } from "fs";
import { fileURLToPath } from "url";
import { resolve } from "path";
import { dirname } from "@angular/compiler-cli";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);
const targetPath = resolve(__dirname, "../src/environments/environment.ts");

const envConfigFile = `export const environment = {
  production: false,
  apiUrl: '${process.env.API_URL || "http://localhost:5050"}'
};
`;

writeFileSync(targetPath, envConfigFile);
