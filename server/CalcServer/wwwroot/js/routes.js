async function getHistory() {
    try {
        const response = await fetch('/api/calculator/history', {
            method: 'GET',
        });

        if (!response.ok) {
            return { error: `Ошибка: ${response.status}` };
        }

        return await response.json();
    } catch (e) {
        console.error('Ошибка получения истории:', e);
        return { error: e.message || 'Сеть недоступна' };
    }
}

async function PostCalculate(expr) {
    try {
        const response = await fetch('/api/calculator/calculate', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                expression: expr,
            }),
        });

        const data = await response.json();

        if (!response.ok) {
            return { error: data.error || response.statusText || `Ошибка: ${response.status}` };
        }

        return data;
    } catch (error) {
        console.error(error);
        return { error: error.message || 'Сеть недоступна' };
    }
}
