/* eslint-disable max-lines */
import { DateTime } from '../AppConstants/DateTime';

Date.prototype.toJSON = function() {
    //use custom implementation for better performance
    const year = this.getUTCFullYear().toString();
    let month = (this.getUTCMonth() + 1).toString();
    let day = this.getUTCDate().toString();
    let hh = this.getUTCHours().toString();
    let mm = this.getUTCMinutes().toString();
    let ss = this.getUTCSeconds().toString();

    if (month.length === 1) month = '0' + month;
    if (day.length === 1) day = '0' + day;
    if (hh.length === 1) hh = '0' + hh;
    if (mm.length === 1) mm = '0' + mm;
    if (ss.length === 1) ss = '0' + ss;

    //'YYYY-MM-DDTHH:MM:SS.Z'
    return `${year}-${month}-${day}T${hh}:${mm}:${ss}Z`;
};

type FormatPatternData = {
    regExp: RegExp,
    patterns: string[]
};

export default class DateTimeService {
    static ISO_8601: RegExp = /^\d{4}(-\d\d(-\d\d(T\d\d:\d\d(:\d\d)?(\.\d+)?(([+-]\d\d:\d\d)|Z)?)?)?)?$/i;
    static ISO_8601_date: RegExp = /^\d{4}-\d\d-\d\d(T\d\d:\d\d(:\d\d)?(\.\d+)?(([+-]\d\d:\d\d)|Z)?)?$/i;
    static regExpDateMask: RegExp = /(YYYY|YY|MMMM|MMM|MM|M|DD|D|dd|d|HH|H|mm|ss|SSS)/g;
    static monthNames: string[] = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];
    static weekDayNames2Letters: string[] = ['Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa', 'Su'];

    //#region NEW API

    static min(...values: (Date | null)[]) : Date {
        const filteredValues = values.filter(x => !!x) as Date[];
        if (!filteredValues.length) throw new Error('Values is an empty array');

        return filteredValues.reduce(function (a, b) {
            return a < b ? a : b;
        });
    }

    static max(...values: (Date | null)[]): Date {
        const filteredValues = values.filter(x => !!x) as Date[];
        if (!filteredValues.length) throw new Error('Values is an empty array');

        return filteredValues.reduce(function (a, b) {
            return a > b ? a : b;
        });
    }

    static distinct(dates: Date[]) {
        const set = new Set(dates.map((d) => d.getTime()));
        return [...set].map((x) => new Date(x));
    }

    static parseUiDate(value: string): Date {
        return this.parse(value, DateTime.viewDateFormat);
    }

    static parseUiDateTime(value: string): Date {
        return this.parse(value, DateTime.viewFullFormat);
    }

    static parseUiMinutes(value: string): number {
        return this.parseUiSeconds(value) / 60;
    }

    static parseUiSeconds(value: string): number {
        if (!value) return NaN;
        const timeParts = value.split(':');
        if (timeParts.length > 2) return NaN;
        const hours = +timeParts[0];
        const minutes = +timeParts[1];

        let result = (hours * 60 + minutes) * 60;
        if (value.charAt(0) === '-') result = result * -1;

        return result;
    }

    static parseUiTime(date: Date, value: string): Date {
        if (!(date instanceof Date)) throw new Error(`Expected date, but found: ${typeof date}: ${date}`);

        const timeParts = value.split(':');
        const hours = +timeParts[0];
        const minutes = +timeParts[1];
        const originDate = DateTimeService.date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());
        originDate.setUTCMinutes(minutes);
        originDate.setUTCHours(hours);

        return originDate;
    }

    static toUiDate(value: Date): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        let d = value.getUTCDate().toString();
        let m = (value.getUTCMonth() + 1).toString();

        d = d.length === 1 ? '0' + d : d;
        m = m.length === 1 ? '0' + m : m;

        return d + '.' + m + '.' + value.getUTCFullYear();
    }

    static toUiDateShortYear(value: Date): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        let d = value.getUTCDate().toString();
        let m = (value.getUTCMonth() + 1).toString();
        const y = value.getFullYear().toString().substr(-2);

        d = d.length === 1 ? '0' + d : d;
        m = m.length === 1 ? '0' + m : m;

        return d + '.' + m + '.' + y;
    }

    static toUiMonth(value: Date): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        let m = (value.getUTCMonth() + 1).toString();
        m = m.length === 1 ? '0' + m : m;

        return m + '.' + value.getUTCFullYear();
    }

    static toUiDateTime(value: Date): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        if (isNaN(value.getTime())) return '~';
        let d = value.getUTCDate().toString();
        let m = (value.getUTCMonth() + 1).toString();
        let h = value.getUTCHours().toString();
        let mm = value.getUTCMinutes().toString();

        d = d.length === 1 ? '0' + d : d;
        m = m.length === 1 ? '0' + m : m;
        h = h.length === 1 ? '0' + h : h;
        mm = mm.length === 1 ? '0' + mm : mm;

        return d + '.' + m + '.' + value.getUTCFullYear() + ' ' + h + ':' + mm;
    }

    static toUiClientShortDate(value: Date): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        const d = value.getDate().toString();
        const mIndex = value.getMonth();
        const m = monthNames[mIndex];

        return m + `'` + d;
    }

    static toUiClientShortDateTime(value: Date): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
        let d = value.getDate().toString();
        const mIndex = value.getMonth();
        let m = monthNames[mIndex];
        let h = value.getHours().toString();
        let mm = value.getMinutes().toString();

        d = d.length === 1 ? '0' + d : d;
        m = m.length === 1 ? '0' + m : m;
        h = h.length === 1 ? '0' + h : h;
        mm = mm.length === 1 ? '0' + mm : mm;

        const isToday = new Date().toDateString() === value.toDateString();
        return (isToday ? '' : d + ' ' + m + ' ') + h + ':' + mm;
    }

    static toUiTime(value: Date, useSwissTime?: boolean): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        if (isNaN(value.getTime())) return '~';
        let h = value.getUTCHours().toString();
        let mm = value.getUTCMinutes().toString();

        h = h.length === 1 ? '0' + h : h;
        mm = mm.length === 1 ? '0' + mm : mm;

        return h + ':' + mm + (useSwissTime ? '.' : '');
    }

    static toUiTimeSeconds(value: Date, useSwissTime?: boolean): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        if (isNaN(value.getTime())) return '~';
        let h = value.getUTCHours().toString();
        let mm = value.getUTCMinutes().toString();
        let ss = value.getUTCSeconds().toString();

        h = h.length === 1 ? '0' + h : h;
        mm = mm.length === 1 ? '0' + mm : mm;
        ss = ss.length === 1 ? '0' + ss : ss;

        return h + ':' + mm + ':' + ss + (useSwissTime ? '.' : '');
    }

    static toUiTimestamp(value: Date, showMS?: boolean): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        let d = value.getUTCDate().toString();
        let m = (value.getUTCMonth() + 1).toString();
        let h = value.getUTCHours().toString();
        let mm = value.getUTCMinutes().toString();
        let s = value.getUTCSeconds().toString();
        let ms = showMS ? `.${value.getUTCMilliseconds()}` : '';

        d = d.length === 1 ? `0${d}` : d;
        m = m.length === 1 ? `0${m}` : m;
        h = h.length === 1 ? `0${h}` : h;
        mm = mm.length === 1 ? `0${mm}` : mm;
        s = s.length === 1 ? `0${s}` : s;
        return `${d}.${m}.${value.getUTCFullYear()} ${h}:${mm}:${s}${ms}`;
    }

    static toUiSeconds(seconds: number): string {
        const sec = Math.trunc(seconds);
        const isNegative = sec < 0;
        const positiveSec = isNegative ? -sec : sec;
        const h = Math.trunc(positiveSec / (60 * 60));
        const m = Math.trunc(positiveSec / 60) - h * 60;
        let mm = m.toString();
        let hh = h.toString();

        mm = mm.length === 1 ? '0' + mm : mm;
        hh = hh.length === 1 ? '0' + hh : hh;

        return (isNegative ? '-' : '') + hh + ':' + mm;
    }

    static toUiMiliseconds(miliseconds: number): string {
        const sec = Math.trunc(miliseconds / 1000);
        const isNegative = sec < 0;
        const h = Math.abs(Math.trunc(sec / (60 * 60)));
        const m = Math.abs(Math.trunc(sec / 60) - h * 60);
        const s = Math.abs(Math.trunc(sec - h * 60 * 60 - m * 60));
        let mm = m.toString();
        let hh = h.toString();
        let ss = s.toString();

        mm = mm.length === 1 ? '0' + mm : mm;
        hh = hh.length === 1 ? '0' + hh : hh;
        ss = ss.length === 1 ? '0' + ss : ss;

        return (isNegative ? '-' : '') + hh + ':' + mm + ':' + ss;
    }

    static toUiDiffTime(to: Date, from: Date) {
        const diffMinutes = this.diffMinutes(to, from);
        if (diffMinutes < 60) {
            return Math.round(diffMinutes) + 'm';
        }

        const diffHours = this.diffHours(to, from);
        if (diffHours < 24) {
            return Math.round(diffHours) + 'h';
        }

        return Math.round(this.diffDays(to, from)) + 'd';
    }

    static toDiffTime(diffInMinutes: number, addColon: boolean = false) {
        const diffAbs = Math.abs(diffInMinutes);
        const hours = Math.ceil(diffAbs / 60);
        const hh = hours.toString().length === 1 ? '0' + hours : hours.toString();
        const minutes = diffAbs - hours * 60;
        const mm = minutes.toString().length === 1 ? '0' + minutes : minutes.toString();

        return (diffInMinutes > 0 ? '+' : '-') + hh + (addColon ? ':' : '') + mm;
    }

    static toCustomUiFormat(value: Date, format: string): string {
        if (!(value instanceof Date)) throw new Error(`Expected date, but found: ${typeof value}: ${value}`);

        return this.format(value, format);
    }

    static toJsonFormat(date: Date): string {
        const month = date.getUTCMonth() + 1;
        const dayOfMOnth = date.getUTCDate();

        return `${date.getUTCFullYear()}-${month < 10 ? '0' + month : month.toString()}-${dayOfMOnth < 10 ? '0' + dayOfMOnth : dayOfMOnth.toString()}`;
    }

    static isSameDate(d1: Date | null, d2: Date | null): boolean {
        if ((d1 || null) === (d2 || null)) return true;
        if (!d1 || !d2) return false;

        return d1.getUTCFullYear() === d2.getUTCFullYear() && d1.getUTCMonth() === d2.getUTCMonth() && d1.getUTCDate() === d2.getUTCDate();
    }

    static isSameMonth(d1: Date, d2: Date): boolean {
        return d1.getUTCFullYear() === d2.getUTCFullYear() && d1.getUTCMonth() === d2.getUTCMonth();
    }

    static isSameYear(d1: Date, d2: Date): boolean {
        return d1.getUTCFullYear() === d2.getUTCFullYear();
    }

    static isAfterDate(d1: Date, d2: Date): boolean {
        if (d1.getUTCFullYear() > d2.getUTCFullYear()) return true;
        if (d1.getUTCFullYear() < d2.getUTCFullYear()) return false;

        if (d1.getUTCMonth() > d2.getUTCMonth()) return true;
        if (d1.getUTCMonth() < d2.getUTCMonth()) return false;

        return d1.getUTCDate() > d2.getUTCDate();
    }

    static isAfterDateOrEqual(d1: Date, d2: Date): boolean {
        if (d1.getUTCFullYear() > d2.getUTCFullYear()) return true;
        if (d1.getUTCFullYear() < d2.getUTCFullYear()) return false;

        if (d1.getUTCMonth() > d2.getUTCMonth()) return true;
        if (d1.getUTCMonth() < d2.getUTCMonth()) return false;

        return d1.getUTCDate() >= d2.getUTCDate();
    }

    static isBeforeDate(d1: Date, d2: Date): boolean {
        if (d1.getUTCFullYear() < d2.getUTCFullYear()) return true;
        if (d1.getUTCFullYear() > d2.getUTCFullYear()) return false;

        if (d1.getUTCMonth() < d2.getUTCMonth()) return true;
        if (d1.getUTCMonth() > d2.getUTCMonth()) return false;

        return d1.getUTCDate() < d2.getUTCDate();
    }

    static isBeforeDateOrEqual(d1: Date, d2: Date): boolean {
        if (d1.getUTCFullYear() < d2.getUTCFullYear()) return true;
        if (d1.getUTCFullYear() > d2.getUTCFullYear()) return false;

        if (d1.getUTCMonth() < d2.getUTCMonth()) return true;
        if (d1.getUTCMonth() > d2.getUTCMonth()) return false;

        return d1.getUTCDate() <= d2.getUTCDate();
    }

    static isBeforeDateTime(d1: Date, d2: Date): boolean {
        if (d1.getUTCFullYear() < d2.getUTCFullYear()) return true;
        if (d1.getUTCFullYear() > d2.getUTCFullYear()) return false;

        if (d1.getUTCMonth() < d2.getUTCMonth()) return true;
        if (d1.getUTCMonth() > d2.getUTCMonth()) return false;

        if (d1.getUTCDate() < d2.getUTCDate()) return true;
        if (d1.getUTCDate() > d2.getUTCDate()) return false;

        if (d1.getUTCHours() < d2.getUTCHours()) return true;
        if (d1.getUTCHours() > d2.getUTCHours()) return false;

        if (d1.getUTCMinutes() < d2.getUTCMinutes()) return true;
        if (d1.getUTCMinutes() > d2.getUTCMinutes()) return false;

        return d1.getUTCSeconds() < d2.getUTCSeconds();
    }

    static compare(d1: Date, d2: Date): number {
        return d1.getTime() - d2.getTime();
    }

    static isValidDate(d: Date) {
        return !isNaN(d.getTime());
    }

    static diffMonths(dateFrom: Date, dateTo: Date) {
        const months = dateTo.getUTCFullYear() * 12 + dateTo.getUTCMonth() - dateFrom.getFullYear() * 12 - dateFrom.getUTCMonth();
        return months;
    }

    static diffDays(to: Date, from: Date): number {
        const date1WithoutTime = this.withTime(to, 0, 0);
        const date2WithoutTime = this.withTime(from, 0, 0);
        const diff = date1WithoutTime.getTime() - date2WithoutTime.getTime();
        return Math.ceil(diff / (24 * 60 * 60 * 1000));
    }

    static diffSeconds(to: Date, from: Date): number {
        const diff = to.getTime() - from.getTime();
        return Math.ceil(diff / 1000);
    }

    static diffMinutes(to: Date, from: Date): number {
        const diff = to.getTime() - from.getTime();
        return Math.ceil(diff / (60 * 1000));
    }

    static diffHours(to: Date, from: Date): number {
        const diff = to.getTime() - from.getTime();
        return diff / (60 * 60 * 1000);
    }

    static addSeconds(date: Date, seconds: number): Date {
        return new Date(date.getTime() + seconds * 1000);
    }

    static addMinutes(date: Date, minutes: number): Date {
        return new Date(date.getTime() + minutes * 60 * 1000);
    }

    static addDays(date: Date, days: number): Date {
        return new Date(date.getTime() + days * 24 * 60 * 60 * 1000);
    }

    static addMonths(date: Date, months: number): Date {
        return this.date(date.getUTCFullYear(), date.getUTCMonth() + months, date.getUTCDate());
    }

    static addYears(date: Date, years: number): Date {
        return this.date(date.getUTCFullYear() + years, date.getUTCMonth(), date.getUTCDate());
    }

    static addTime(date: Date, hours: number, minutes?: number): Date {
        let offset = hours * 60 * 60 * 1000;
        if (minutes) {
            offset += minutes * 60 * 1000;
        }

        return new Date(date.getTime() + offset);
    }

    static addDiffToLt(date: Date, sourceOffset: string) {
        const offset = sourceOffset.indexOf(':') ? sourceOffset.replace(':', '') : sourceOffset;
        const signChar = offset[0];
        const sign = signChar === '-' ? -1 : 1;
        const timeDiff = offset.substring(signChar === '-' || signChar === '+' ? 1 : 0);
        if (timeDiff.length !== 4) {
            throw new Error('Invalid time offset, expected format +HHMM, actual: ' + offset);
        }
        const hh = sign * +timeDiff.substring(0, 2);
        const mm = sign * +timeDiff.substring(2, 4);

        return DateTimeService.addTime(date, hh, mm);
    }

    static today(): Date {
        const today = new Date();
        return DateTimeService.date(today.getUTCFullYear(), today.getUTCMonth(), today.getUTCDate());
    }

    static currentTime() {
        return new Date().getTime();
    }

    static trimTime(dateWithTime: Date) {
        return DateTimeService.date(dateWithTime.getUTCFullYear(), dateWithTime.getUTCMonth(), dateWithTime.getUTCDate());
    }

    static range(dateFrom: Date, dateTo: Date): Array<Date> {
        const days: Date[] = [];
        let date = dateFrom;
        days.push(date);
        while (date.getTime() < dateTo.getTime()) {
            date = new Date(date.getTime() + 24 * 60 * 60 * 1000);
            days.push(date);
        }
        return days;
    }

    static fromString(value: string | Date): Date {
        if (typeof value === 'string') return new Date(Date.parse(value));

        return value;
    }

    static withTime(date: Date, hours: number, minutes: number): Date {
        const d = DateTimeService.date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());

        return DateTimeService.addTime(d, hours, minutes);
    }

    static date(year: number, month: number, day: number): Date {
        return new Date(Date.UTC(year, month, day, 0, 0, 0, 0));
    }

    static now(): Date {
        return new Date();
    }

    static startOfMonth(): Date {
        const now = this.now();
        return this.date(now.getUTCFullYear(), now.getUTCMonth(), 1);
    }

    static toFirstDayOfYear(date: Date): Date {
        return this.date(date.getUTCFullYear(), 0, 1);
    }

    static toFirstDayOfMonth(date: Date): Date {
        return this.date(date.getUTCFullYear(), date.getUTCMonth(), 1);
    }

    static toLastDayOfMonth(date: Date): Date {
        return this.date(date.getUTCFullYear(), date.getUTCMonth() + 1, 0);
    }

    static startOfDate(date: Date): Date {
        return this.date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate());
    }

    static toFirstDayOfWeek(date: Date): Date {
        let dayOfWeek = date.getUTCDay() - 1;
        if (dayOfWeek < 0) {
            dayOfWeek = 6;
        }

        return this.date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate() - dayOfWeek);
    }

    static toLastDayOfWeek(date: Date): Date {
        let dayOfWeek = date.getUTCDay() - 1;
        if (dayOfWeek < 0) {
            dayOfWeek = 6;
        }

        return this.date(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate() - dayOfWeek + 6);
    }

    static getDateIndexInRange(date: Date, dateFrom: Date, dateTo?: Date): number {
        if (date < dateFrom) return -1;
        if (dateTo && date > dateTo) return -1;
        return this.diffDays(date, dateFrom);
    }

    static calculateAge(birthday: Date, atDate: Date): number {
        const ageDifMs = atDate.getTime() - birthday.getTime();
        const ageDate = new Date(ageDifMs); // miliseconds from epoch
        return Math.abs(ageDate.getUTCFullYear() - 1970);
    }

    static isIntersects(r1: { from: Date; to: Date }, r2: { from: Date; to: Date }) {
        return r1.to >= r2.from && r1.from <= r2.to;
    }

    static asRestString(date: Date) {
        return '/' + date.getUTCFullYear() + '/' + (date.getUTCMonth() + 1) + '/' + date.getUTCDate();
    }

    //#endregion

    static readonly patternCache: { [key: string]: FormatPatternData } = {};

    static readonly patternsToRegex: { [key: string]: string } = {
        YYYY: '(\\d{4})',
        MMMM: '(\\S+)',
        MM: '(\\d{2})',
        M: '(\\d{1,2})',
        DD: '(\\d{2})',
        D: '(\\d{1,2})',
        HH: '(\\d{2})',
        H: '(\\d{1,2})',
        mm: '(\\d{2})',
        ss: '(\\d{2})',
        SSS: '(\\d{3})'
    };

    static parse(value: string, format: string): Date {
        if (typeof value !== 'string') return this.getNanDate();

        let year = 1;
        let month = 0;
        let day = 1;
        let hour = 0;
        let minute = 0;
        let second = 0;
        let milliseconds = 0;
        let pattern = DateTimeService.patternCache[format];

        if (!pattern) {
            const patterns: string[] = [];

            const regexpText = format.replace(this.regExpDateMask, (_, pattern) => {
                patterns.push(pattern);
                return DateTimeService.patternsToRegex[pattern];
            });

            const regexp = new RegExp(regexpText);
            pattern = {
                regExp: regexp,
                patterns: patterns
            };
            DateTimeService.patternCache[format] = pattern;
        }

        const match = value.match(pattern.regExp);
        if (match) {
            for (let i = 1; i < match.length; i++) {
                const foundValue = pattern.patterns[i - 1];
                const valuePart = match[i];

                switch (foundValue) {
                    case 'YYYY':
                        year = this._parseSafeInteger(valuePart, 9999);
                        break;
                    case 'M':
                    case 'MM':
                        month = this._parseSafeInteger(valuePart, 12) - 1;
                        break;
                    case 'MMMM':
                        month = this.monthNames.indexOf(valuePart);
                        if (month === -1) throw new Error('Invalid month name');
                        break;
                    case 'D':
                    case 'DD':
                        day = this._parseSafeInteger(valuePart, 31);
                        break;
                    case 'H':
                    case 'HH':
                        hour = this._parseSafeInteger(valuePart, 23);
                        break;
                    case 'mm':
                        minute = this._parseSafeInteger(valuePart, 59);
                        break;
                    case 'ss':
                        second = this._parseSafeInteger(valuePart, 59);
                        break;
                    case 'SSS':
                        milliseconds = this._parseSafeInteger(valuePart, 999);
                        break;
                }
            }
        } else {
            return this.getNanDate();
        }

        const newDate = new Date(Date.UTC(year, month, day, hour, minute, second, milliseconds));
        const newDateDay = newDate.getUTCDate();
        if (newDateDay === day) {
            return newDate;
        }

        return this.getNanDate();
    }

    static format(date: Date, format: string = DateTime.viewDateFormat): string {
        let result = format;
        let dayOfWeek = date.getUTCDay() - 1;
        result = result.replace(this.regExpDateMask, (match: string) => {
            switch (match) {
                case 'DD':
                    return date.getUTCDate() < 10 ? '0' + date.getUTCDate() : date.getUTCDate().toString();
                case 'D':
                    return date.getUTCDate().toString();
                case 'dd':
                    if (dayOfWeek < 0) {
                        dayOfWeek = 6;
                    }
                    return this.weekDayNames2Letters[dayOfWeek];
                case 'MMMM':
                    return this.monthNames[date.getUTCMonth()];
                case 'MMM':
                    return this.monthNames[date.getUTCMonth()].substring(0, 3).toUpperCase();
                case 'MM':
                    return date.getUTCMonth() + 1 < 10 ? '0' + (date.getUTCMonth() + 1).toString() : (date.getUTCMonth() + 1).toString();
                case 'M':
                    return (date.getUTCMonth() + 1).toString();
                case 'YYYY':
                    return date.getUTCFullYear().toString();
                case 'YY':
                    return date.getUTCFullYear().toString().substr(-2);
                case 'HH':
                    return date.getUTCHours() < 10 ? '0' + date.getUTCHours() : date.getUTCHours().toString();
                case 'H':
                    return date.getUTCHours().toString();
                case 'mm':
                    return date.getUTCMinutes() < 10 ? '0' + date.getUTCMinutes() : date.getUTCMinutes().toString();
                case 'ss':
                    return date.getUTCSeconds() < 10 ? '0' + date.getUTCSeconds() : date.getUTCSeconds().toString();

                default:
                    return match;
            }
        });
        return result;
    }

    static toQueryParam(value: Date) {
        return value.toJSON();
    }

    static getRefYear(): number {
        const date = DateTimeService.now();
        return date.getUTCMonth() <= 6 ? DateTimeService.addYears(date, -1).getUTCFullYear() : date.getUTCFullYear();
    }

    static getZeroDate() {
        return new Date(0);
    }

    static getNanDate(): Date {
        return new Date(NaN);
    }

    static getStartOfDay(year: number, month: number, day: number): Date {
        const result = this.getZeroDate();
        result.setUTCFullYear(year);
        result.setUTCMonth(month);
        result.setUTCDate(day);
        return result;
    }

    private static _parseSafeInteger(value: string, maxValue: number) {
        const parsedPart = parseInt(value, 10);
        if (isNaN(parsedPart) || parsedPart < 0 || !Number.isInteger(parsedPart)) {
            throw new Error('Failed parse ' + value + ' exception, expected format [0-9]');
        }
        if (parsedPart > maxValue) {
            return NaN;
        }

        return parsedPart;
    }

    static getAllowedChangesDate(): Date {
        const dateNow = DateTimeService.today();
        const currentDay = dateNow.getUTCDate();
        const startOfMonth = DateTimeService.toFirstDayOfMonth(dateNow);
        const startOfPrevMonth = DateTimeService.addMonths(startOfMonth, -1);

        return currentDay >= 3 ? startOfMonth : startOfPrevMonth;
    }

    public static getShortRelatedTime(time: Date, relatedDate: Date, showSwissTime?: boolean) {
        const referenceDateTimeDiff = DateTimeService.diffDays(time, relatedDate);

        const diffStr = isNaN(referenceDateTimeDiff) ? '~' : referenceDateTimeDiff === 0 ? '' : (referenceDateTimeDiff > 0 ? '(+' : '(') + referenceDateTimeDiff + ') ';
        const timeString = DateTimeService.toUiTime(time, showSwissTime);

        return diffStr + timeString;
    }

    public static getShortRelatedTimeWithSeconds(time: Date, relatedDate: Date, showSwissTime?: boolean) {
        const referenceDateTimeDiff = DateTimeService.diffDays(time, relatedDate);

        const diffStr = isNaN(referenceDateTimeDiff) ? '~' : referenceDateTimeDiff === 0 ? '' : (referenceDateTimeDiff > 0 ? '(+' : '(') + referenceDateTimeDiff + ') ';
        const timeString = DateTimeService.toUiTimeSeconds(time, showSwissTime);

        return diffStr + timeString;
    }

    public static getDaysInMonth(date: Date) {
        return new Date(date.getFullYear(), date.getMonth() + 1, 0).getDate();
    }

    public static rangesIntersection(dateFromA: Date, dateToA: Date, dateFromB: Date, dateToB: Date) {
        if ((dateToA < dateFromB) || (dateFromA > dateToB)) {
            return null;
        }

        const result = {} as { from: Date, to: Date };
        result.from = dateFromA <= dateFromB ? dateFromB : dateFromA;
        result.to = dateToA <= dateToB ? dateToA : dateToB;

        return result;
    }
}
