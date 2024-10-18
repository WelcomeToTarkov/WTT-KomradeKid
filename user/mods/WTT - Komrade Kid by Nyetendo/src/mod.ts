/* eslint-disable @typescript-eslint/naming-convention */

import * as fs from "fs";
import * as path from "path";
import { Props } from "@spt/models/eft/common/tables/ITemplateItem";
import { DependencyContainer } from "tsyringe";
import { IPostDBLoadMod } from "@spt/models/external/IPostDBLoadMod";
import { IPreSptLoadMod } from "@spt/models/external/IPreSptLoadMod";
import { LogTextColor } from "@spt/models/spt/logging/LogTextColor";
import type { GameController } from "@spt/controllers/GameController";
import type { IEmptyRequestData } from "@spt/models/eft/common/IEmptyRequestData";
// WTT imports
import { WTTInstanceManager } from "./WTTInstanceManager";

import { CustomItemService } from "./CustomItemService";

import * as config from "../config/config.json";
import { ItemType } from "@spt/models/eft/common/tables/ITemplateItem";


export interface GameboyCartridgeProps extends Props {
    RomName?: string;
}

class KomradeKid
    implements IPreSptLoadMod, IPostDBLoadMod {
    private Instance: WTTInstanceManager = new WTTInstanceManager();
    private version: string;
    private modName = "WTT - Komrade Kid by Nyetendo";

    private CustomItemService: CustomItemService = new CustomItemService();

    debug = false;

    showSplashScreen = true;

    // Anything that needs done on preSptLoad, place here.
    public preSptLoad(container: DependencyContainer): void {
        // Initialize the instance manager DO NOTHING ELSE BEFORE THIS
        this.Instance.preSptLoad(container, this.modName);
        this.Instance.debug = this.debug;
        // EVERYTHING AFTER HERE MUST USE THE INSTANCE
        this.getVersionFromJson();

        if (this.showSplashScreen)
        {
            this.displayCreditBanner();

        }

        this.CustomItemService.preSptLoad(this.Instance);

    }

    // Anything that needs done on postDBLoad, place here.
    postDBLoad(container: DependencyContainer): void {
        // Initialize the instance manager DO NOTHING ELSE BEFORE THIS
        this.Instance.postDBLoad(container);
        // EVERYTHING AFTER HERE MUST USE THE INSTANCE
        // Ensure that the custom item is correctly added after the database load.
        const itemsDB = this.Instance.database.templates?.items;

        if (itemsDB) {
            itemsDB["66e42bd851fa456a1ee37885"] = {
                _id: "66e42bd851fa456a1ee37885",
                _name: "CustomUsableItem",
                _parent: "566162e44bdc2d3f298b4573",
                _type: ItemType.NODE,
                _props: {
                }
            };

            itemsDB["66f16b85ed966fb78f5563d8"] = {
                _id: "66f16b85ed966fb78f5563d8",
                _name: "GameBoyModTemplateType",
                _parent: "566162e44bdc2d3f298b4573",
                _type: ItemType.NODE,
                _props: {
                    RomName: "",
                    CartridgeImage: "",
                    AccessoryType: ""
                } as GameboyCartridgeProps
              };

              itemsDB["66f17b4cb59dbccbf12990e6"] = {
                _id: "66f17b4cb59dbccbf12990e6",
                _name: "GameboyCartridge",
                _parent: "66f16b85ed966fb78f5563d8",
                _type: ItemType.NODE,
                _props: {
                }
            };
            itemsDB["6704271a4cc9e20c610eb120"] = {
                _id: "6704271a4cc9e20c610eb120",
                _name: "GameboyAccessory",
                _parent: "66f16b85ed966fb78f5563d8",
                _type: ItemType.NODE,
                _props: {
                }
            };
        }
                
        this.CustomItemService.postDBLoad();

        this.colorLog(
            `[${this.modName}] Database: Loading complete.`,
            "green"
        );
    }
    private getVersionFromJson(): void {
        const packageJsonPath = path.join(__dirname, "../package.json");

        fs.readFile(packageJsonPath, "utf-8", (err, data) => {
            if (err) {
                console.error("Error reading file:", err);
                return;
            }

            const jsonData = JSON.parse(data);
            this.version = jsonData.version;
        });
    }

    public colorLog(message: string, color: string) {
        const colorCodes = {
            red: "\x1b[31m",
            green: "\x1b[32m",
            yellow: "\x1b[33m",
            blue: "\x1b[34m",
            magenta: "\x1b[35m",
            cyan: "\x1b[36m",
            white: "\x1b[37m"
        };
      
        const resetCode = "\x1b[0m";
        const colorCode = colorCodes[color as keyof typeof colorCodes] || "\x1b[37m"; // Default to white if color is invalid.
        console.log(`${colorCode}${message}${resetCode}`); // Log the colored message here
    }

    private displayCreditBanner(): void {
        this.colorLog(` 
             ___________________
            |  _______________  |
            | |   NYETENDO    | |
            | |  KOMRADE KID  | |
            | |               | |
            | | ▒▒▒▒▒▒▒▒▒▒▒▒▒ | |
            | | ▒▒▒▒▒▒▒▒▒▒▒▒▒ | |
            | | ▒▒▒▒▒▒▒▒▒▒▒▒▒ | |
            | | ▒▒▒▒▒▒▒▒▒▒▒▒▒ | |
            | |_______________| |
            |  _____       ___  |
            | |__|__|  ___|___| |
            | |__|__| |___|     |
            |   ____     ____   |
            |  |____|   |____|  |
            |___________________|
            | BY: Groovey       |
            |       & WTT Team  |
            |___________________|
            `,
            "magenta"
        );
        this.colorLog(
            "                   Special Thanks To: Tron, Rexana, EpicRangeTime, Krackasaurus, CJ, and many more!",
            "magenta"
        );
        this.colorLog(
            `
            
                                                                                         NO WESTERN GAMES ALLOWED!!`,
            "magenta"
        );
    }
}

module.exports = { mod: new KomradeKid() };
