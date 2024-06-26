import { Select } from 'antd';
import React, { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react';

interface ICellEditorReactComp {
    values: {
        label: string;
        value: string;
    }[];
    getValue: () => string[];
    value: {
        label: string;
        value: string;
    };
}

const DataGridDropdownCell = forwardRef((props: ICellEditorReactComp, ref) => {
    const [value, setValue] = useState<string>(props.value?.value);
    const refInput = useRef<React.RefObject<unknown> | null>(null);

    useEffect(() => {
        // focus on input
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        (refInput.current as any)?.focus();
    }, []);

    useImperativeHandle(ref, () => {
        return {
            getValue: () => {
                const result = props.values.find(x => x.value === value);
                return result;
            }
        };
    });

    const filterOption = (input: string, option?: { label: string; value: string }) => (option?.label ?? '').toLowerCase().includes(input.toLowerCase());

    return (
        <Select showSearch filterOption={filterOption} style={{ width: '100%', height: '100%' }} value={value} onChange={setValue}>
            {props.values.map((x) => (
                <Select.Option key={x.value} value={x.value}>
                    {x.label}
                </Select.Option>
            ))}
        </Select>
    );
});

export default DataGridDropdownCell;
