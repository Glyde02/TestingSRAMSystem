/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.c
  * @brief          : Main program body
  ******************************************************************************
  * @attention
  *
  * Copyright (c) 2022 STMicroelectronics.
  * All rights reserved.
  *
  * This software is licensed under terms that can be found in the LICENSE file
  * in the root directory of this software component.
  * If no LICENSE file comes with this software, it is provided AS-IS.
  *
  ******************************************************************************
  */
/* USER CODE END Header */
/* Includes ------------------------------------------------------------------*/
#include "main.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Private typedef -----------------------------------------------------------*/
/* USER CODE BEGIN PTD */

/* USER CODE END PTD */

/* Private define ------------------------------------------------------------*/
/* USER CODE BEGIN PD */
/* USER CODE END PD */

/* Private macro -------------------------------------------------------------*/
/* USER CODE BEGIN PM */

/* USER CODE END PM */

/* Private variables ---------------------------------------------------------*/
UART_HandleTypeDef huart2;

/* USER CODE BEGIN PV */
int flag_irq = 0;
int time_irq = 0;
const uint8_t digits[]   = { 0xFF, 0xCF, 0xF9, 0xC9 };
												//		00 		01		10		11
uint8_t array[]   = { 0x00, 0x00, 0x00, 0x00 };
int data[8];
int row = 0;
int col = 0;
int time = 0;
/* USER CODE END PV */

/* Private function prototypes -----------------------------------------------*/
void SystemClock_Config(void);
static void MX_GPIO_Init(void);
static void MX_USART2_UART_Init(void);
/* USER CODE BEGIN PFP */

/* USER CODE END PFP */

/* Private user code ---------------------------------------------------------*/
/* USER CODE BEGIN 0 */
void write_digit(uint8_t pos, uint8_t digit) {
  int j = 0;
	while(j < 8) {
	  if (((digit << j) & 0x80) != 0) {
		  HAL_GPIO_WritePin(Data_GPIO_Port, Data_Pin, GPIO_PIN_SET);
		} else {
		  HAL_GPIO_WritePin(Data_GPIO_Port, Data_Pin, GPIO_PIN_RESET);
		}
		HAL_GPIO_WritePin(Shift_GPIO_Port, Shift_Pin, GPIO_PIN_SET);
		HAL_GPIO_WritePin(Shift_GPIO_Port, Shift_Pin, GPIO_PIN_RESET);
		j++;
	}
	
	j = 0;
	while(j < 8) {
	  if (((pos << j) & 0x80) != 0) {
		  HAL_GPIO_WritePin(Data_GPIO_Port, Data_Pin, GPIO_PIN_SET);
		} else {
		  HAL_GPIO_WritePin(Data_GPIO_Port, Data_Pin, GPIO_PIN_RESET);
		}
		HAL_GPIO_WritePin(Shift_GPIO_Port, Shift_Pin, GPIO_PIN_SET);
		HAL_GPIO_WritePin(Shift_GPIO_Port, Shift_Pin, GPIO_PIN_RESET);
		j++;
	}
	
	HAL_GPIO_WritePin(Latch_GPIO_Port, Latch_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(Latch_GPIO_Port, Latch_Pin, GPIO_PIN_RESET);
}
/* USER CODE END 0 */

/**
  * @brief  The application entry point.
  * @retval int
  */
int main(void)
{
  /* USER CODE BEGIN 1 */

  /* USER CODE END 1 */

  /* MCU Configuration--------------------------------------------------------*/

  /* Reset of all peripherals, Initializes the Flash interface and the Systick. */
  HAL_Init();

  /* USER CODE BEGIN Init */

  /* USER CODE END Init */

  /* Configure the system clock */
  SystemClock_Config();

  /* USER CODE BEGIN SysInit */

  /* USER CODE END SysInit */

  /* Initialize all configured peripherals */
  MX_GPIO_Init();
  MX_USART2_UART_Init();
  /* USER CODE BEGIN 2 */

  /* USER CODE END 2 */

  /* Infinite loop */
  /* USER CODE BEGIN WHILE */
  
  while (1)
  {
		/*
		if(flag_irq && (HAL_GetTick() - time_irq) > 200)
		{
			__HAL_GPIO_EXTI_CLEAR_IT(BTN1_Pin); 
			__HAL_GPIO_EXTI_CLEAR_IT(BTN2_Pin);
			__HAL_GPIO_EXTI_CLEAR_IT(B1_Pin);
			NVIC_ClearPendingIRQ(EXTI15_10_IRQn); 
			NVIC_ClearPendingIRQ(EXTI9_5_IRQn); 
			NVIC_ClearPendingIRQ(EXTI4_IRQn);
			HAL_NVIC_EnableIRQ(EXTI15_10_IRQn); 
			HAL_NVIC_EnableIRQ(EXTI9_5_IRQn);  
			HAL_NVIC_EnableIRQ(EXTI4_IRQn); 			
			
			flag_irq = 0;
		}
		
		
			write_digit(0x01, array[0]);
			write_digit(0x02, array[1]);
			//write_digit(0x02, 0x00);	
			write_digit(0x04, array[2]);
			write_digit(0x08, array[3]);------------------------*/
		
		
		uint8_t Test[] = "Hello World !!!\r\n"; //Data to send
		HAL_UART_Transmit(&huart2,Test,sizeof(Test),10);
		HAL_Delay(1000);
    /* USER CODE END WHILE */

    /* USER CODE BEGIN 3 */
  }
	
  /* USER CODE END 3 */
}

/**
  * @brief System Clock Configuration
  * @retval None
  */
void SystemClock_Config(void)
{
  RCC_OscInitTypeDef RCC_OscInitStruct = {0};
  RCC_ClkInitTypeDef RCC_ClkInitStruct = {0};

  /** Initializes the RCC Oscillators according to the specified parameters
  * in the RCC_OscInitTypeDef structure.
  */
  RCC_OscInitStruct.OscillatorType = RCC_OSCILLATORTYPE_HSI;
  RCC_OscInitStruct.HSIState = RCC_HSI_ON;
  RCC_OscInitStruct.HSICalibrationValue = RCC_HSICALIBRATION_DEFAULT;
  RCC_OscInitStruct.PLL.PLLState = RCC_PLL_ON;
  RCC_OscInitStruct.PLL.PLLSource = RCC_PLLSOURCE_HSI_DIV2;
  RCC_OscInitStruct.PLL.PLLMUL = RCC_PLL_MUL16;
  if (HAL_RCC_OscConfig(&RCC_OscInitStruct) != HAL_OK)
  {
    Error_Handler();
  }

  /** Initializes the CPU, AHB and APB buses clocks
  */
  RCC_ClkInitStruct.ClockType = RCC_CLOCKTYPE_HCLK|RCC_CLOCKTYPE_SYSCLK
                              |RCC_CLOCKTYPE_PCLK1|RCC_CLOCKTYPE_PCLK2;
  RCC_ClkInitStruct.SYSCLKSource = RCC_SYSCLKSOURCE_PLLCLK;
  RCC_ClkInitStruct.AHBCLKDivider = RCC_SYSCLK_DIV1;
  RCC_ClkInitStruct.APB1CLKDivider = RCC_HCLK_DIV2;
  RCC_ClkInitStruct.APB2CLKDivider = RCC_HCLK_DIV1;

  if (HAL_RCC_ClockConfig(&RCC_ClkInitStruct, FLASH_LATENCY_2) != HAL_OK)
  {
    Error_Handler();
  }
}

/**
  * @brief USART2 Initialization Function
  * @param None
  * @retval None
  */
static void MX_USART2_UART_Init(void)
{

  /* USER CODE BEGIN USART2_Init 0 */

  /* USER CODE END USART2_Init 0 */

  /* USER CODE BEGIN USART2_Init 1 */

  /* USER CODE END USART2_Init 1 */
  huart2.Instance = USART2;
  huart2.Init.BaudRate = 115200;
  huart2.Init.WordLength = UART_WORDLENGTH_8B;
  huart2.Init.StopBits = UART_STOPBITS_1;
  huart2.Init.Parity = UART_PARITY_NONE;
  huart2.Init.Mode = UART_MODE_TX_RX;
  huart2.Init.HwFlowCtl = UART_HWCONTROL_NONE;
  huart2.Init.OverSampling = UART_OVERSAMPLING_16;
  if (HAL_UART_Init(&huart2) != HAL_OK)
  {
    Error_Handler();
  }
  /* USER CODE BEGIN USART2_Init 2 */

  /* USER CODE END USART2_Init 2 */

}

/**
  * @brief GPIO Initialization Function
  * @param None
  * @retval None
  */
static void MX_GPIO_Init(void)
{
  GPIO_InitTypeDef GPIO_InitStruct = {0};

  /* GPIO Ports Clock Enable */
  __HAL_RCC_GPIOC_CLK_ENABLE();
  __HAL_RCC_GPIOD_CLK_ENABLE();
  __HAL_RCC_GPIOA_CLK_ENABLE();
  __HAL_RCC_GPIOB_CLK_ENABLE();

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOC, Column0_Pin|Column1_Pin|Column2_Pin|Column3_Pin
                          |Column4_Pin|Column5_Pin|OE_Pin|Row7_Pin
                          |Row8_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOA, CE_Pin|WE_Pin|LD2_Pin|Row0_Pin
                          |Row1_Pin|Row2_Pin|Row3_Pin|Row4_Pin
                          |Row5_Pin|Row6_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin Output Level */
  HAL_GPIO_WritePin(GPIOB, Latch_Pin|Data_Pin|Shift_Pin, GPIO_PIN_RESET);

  /*Configure GPIO pin : B1_Pin */
  GPIO_InitStruct.Pin = B1_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_FALLING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(B1_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pins : Column0_Pin Column1_Pin Column2_Pin Column3_Pin
                           Column4_Pin Column5_Pin OE_Pin Row7_Pin
                           Row8_Pin */
  GPIO_InitStruct.Pin = Column0_Pin|Column1_Pin|Column2_Pin|Column3_Pin
                          |Column4_Pin|Column5_Pin|OE_Pin|Row7_Pin
                          |Row8_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_OD;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOC, &GPIO_InitStruct);

  /*Configure GPIO pins : CE_Pin WE_Pin Row0_Pin Row1_Pin
                           Row2_Pin Row3_Pin Row4_Pin Row5_Pin
                           Row6_Pin */
  GPIO_InitStruct.Pin = CE_Pin|WE_Pin|Row0_Pin|Row1_Pin
                          |Row2_Pin|Row3_Pin|Row4_Pin|Row5_Pin
                          |Row6_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_OD;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOA, &GPIO_InitStruct);

  /*Configure GPIO pin : BTN2_Pin */
  GPIO_InitStruct.Pin = BTN2_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(BTN2_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pin : LD2_Pin */
  GPIO_InitStruct.Pin = LD2_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(LD2_GPIO_Port, &GPIO_InitStruct);

  /*Configure GPIO pins : Data2_Pin Data3_Pin Data4_Pin Data0_Pin
                           Data1_Pin Data5_Pin Data6_Pin Data7_Pin */
  GPIO_InitStruct.Pin = Data2_Pin|Data3_Pin|Data4_Pin|Data0_Pin
                          |Data1_Pin|Data5_Pin|Data6_Pin|Data7_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /*Configure GPIO pins : Latch_Pin Data_Pin Shift_Pin */
  GPIO_InitStruct.Pin = Latch_Pin|Data_Pin|Shift_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_PP;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);

  /*Configure GPIO pin : BTN1_Pin */
  GPIO_InitStruct.Pin = BTN1_Pin;
  GPIO_InitStruct.Mode = GPIO_MODE_IT_RISING;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(BTN1_GPIO_Port, &GPIO_InitStruct);

  /* EXTI interrupt init*/
  HAL_NVIC_SetPriority(EXTI4_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(EXTI4_IRQn);

  HAL_NVIC_SetPriority(EXTI9_5_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(EXTI9_5_IRQn);

  HAL_NVIC_SetPriority(EXTI15_10_IRQn, 0, 0);
  HAL_NVIC_EnableIRQ(EXTI15_10_IRQn);

}

/* USER CODE BEGIN 4 */
void ReadEEPROM(){
	
	HAL_GPIO_WritePin(CE_GPIO_Port, CE_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(WE_GPIO_Port, WE_Pin, GPIO_PIN_SET);
	
	HAL_GPIO_WritePin(Row0_GPIO_Port, Row0_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row1_GPIO_Port, Row1_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row2_GPIO_Port, Row2_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row3_GPIO_Port, Row3_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row4_GPIO_Port, Row4_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row5_GPIO_Port, Row5_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row6_GPIO_Port, Row6_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row7_GPIO_Port, Row7_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row8_GPIO_Port, Row8_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column0_GPIO_Port, Column0_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column1_GPIO_Port, Column1_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column2_GPIO_Port, Column2_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column3_GPIO_Port, Column3_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column4_GPIO_Port, Column4_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column5_GPIO_Port, Column5_Pin, GPIO_PIN_RESET);
	
	//HAL_GPIO_WritePin(Row0_GPIO_Port, Row0_Pin | Row1_Pin | Row2_Pin | Row3_Pin | Row4_Pin | Row5_Pin | Row6_Pin | Row7_Pin | Row8_Pin, GPIO_PIN_RESET);
	//HAL_GPIO_WritePin(Column0_GPIO_Port, Column0_Pin | Column1_Pin | Column2_Pin | Column3_Pin | Column4_Pin | Column5_Pin, GPIO_PIN_RESET);

	/*
		HAL_GPIO_WritePin(GPIOA, Row0_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row1_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row2_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row3_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row4_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row5_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row6_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row7_Pin, GPIO_PIN_RESET);
		HAL_GPIO_WritePin(GPIOA, Row8_Pin, GPIO_PIN_RESET);
	
		HAL_GPIO_WritePin(GPIOC, Column0_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(GPIOC, Column1_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(GPIOC, Column2_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(GPIOC, Column3_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(GPIOC, Column4_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(GPIOC, Column5_Pin, GPIO_PIN_RESET);*/
	
	
	if ((row & 0b000000001) == 1)
		HAL_GPIO_WritePin(Row0_GPIO_Port, Row0_Pin, GPIO_PIN_SET);
	if (((row & 0b000000010) >> 1) == 1)
		HAL_GPIO_WritePin(Row1_GPIO_Port, Row1_Pin, GPIO_PIN_SET);
	if (((row & 0b000000100) >> 2) == 1)
		HAL_GPIO_WritePin(Row2_GPIO_Port, Row2_Pin, GPIO_PIN_SET);
	if (((row & 0b000001000) >> 3) == 1)
		HAL_GPIO_WritePin(Row3_GPIO_Port, Row3_Pin, GPIO_PIN_SET);
	if (((row & 0b000010000) >> 4) == 1)
		HAL_GPIO_WritePin(Row4_GPIO_Port, Row4_Pin, GPIO_PIN_SET);
	if (((row & 0b000100000) >> 5) == 1)
		HAL_GPIO_WritePin(Row5_GPIO_Port, Row5_Pin, GPIO_PIN_SET);
	if (((row & 0b001000000) >> 6) == 1)
		HAL_GPIO_WritePin(Row6_GPIO_Port, Row6_Pin, GPIO_PIN_SET);
	if (((row & 0b010000000) >> 7) == 1)
		HAL_GPIO_WritePin(Row7_GPIO_Port, Row7_Pin, GPIO_PIN_SET);
	if (((row & 0b100000000) >> 8) == 1)
		HAL_GPIO_WritePin(Row8_GPIO_Port, Row8_Pin, GPIO_PIN_SET);
	
	if ((col & 0b000001) == 1)
		HAL_GPIO_WritePin(Column0_GPIO_Port, Column0_Pin, GPIO_PIN_SET);
	if (((col & 0b000010) >> 1) == 1)
		HAL_GPIO_WritePin(Column1_GPIO_Port, Column1_Pin, GPIO_PIN_SET);
	if (((col & 0b000100) >> 2) == 1)
		HAL_GPIO_WritePin(Column2_GPIO_Port, Column2_Pin, GPIO_PIN_SET);
	if (((col & 0b001000) >> 3) == 1)
		HAL_GPIO_WritePin(Column3_GPIO_Port, Column3_Pin, GPIO_PIN_SET);
	if (((col & 0b010000) >> 4) == 1)
		HAL_GPIO_WritePin(Column4_GPIO_Port, Column4_Pin, GPIO_PIN_SET);
	if (((col & 0b100000) >> 5) == 1)
		HAL_GPIO_WritePin(Column5_GPIO_Port, Column5_Pin, GPIO_PIN_SET);
	
	
	//HAL_GPIO_WritePin(CE_GPIO_Port, CE_Pin, GPIO_PIN_SET);
	//HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_SET);
	//HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_RESET);
	
	HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_RESET);
	
	data[0] = HAL_GPIO_ReadPin(Data0_GPIO_Port, Data0_Pin);	
	data[1] = HAL_GPIO_ReadPin(Data1_GPIO_Port, Data1_Pin);	
	data[2] = HAL_GPIO_ReadPin(Data2_GPIO_Port, Data2_Pin);//---------
	//data[3] = HAL_GPIO_ReadPin(Data3_GPIO_Port, Data3_Pin);//---------
	data[3] = HAL_GPIO_ReadPin(Data3_GPIO_Port, Data3_Pin);
	data[4] = HAL_GPIO_ReadPin(Data4_GPIO_Port, Data4_Pin);
	data[5] = HAL_GPIO_ReadPin(Data5_GPIO_Port, Data5_Pin);
	data[6] = HAL_GPIO_ReadPin(Data6_GPIO_Port, Data6_Pin);
	data[7] = HAL_GPIO_ReadPin(Data7_GPIO_Port, Data7_Pin);
	
	if (data[3] == 1){
		HAL_GPIO_WritePin(GPIOA, LD2_Pin, GPIO_PIN_SET);
		
	}
	
	//HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_SET);
	//HAL_GPIO_WritePin(CE_GPIO_Port, CE_Pin, GPIO_PIN_SET);
	
}

void ShowData(){

	for (int i = 0; i < 4; i++){
		if (data[2*i + 1] == 0 && data[2*i] == 0)
			array[i] = digits[0];
		else	if (data[2*i + 1] == 0 && data[2*i] == 1)
			array[i] = digits[1];
		else 	if (data[2*i + 1] == 1 && data[2*i] == 0)
			array[i] = digits[2];
		else 	if (data[2*i + 1] == 1 && data[2*i] == 1)
			array[i] = digits[3];
	}
	
	/*array[0] = 0xC0;
	array[1] = 0xF9;
	array[2] = 0xA4;
	array[3] = 0xB0;*/
}

void ProgramDelay(int count){
	int a;
	int b = 3;
	while(count > 0)
	{
		a = b;
		a = 0;
		count--;
	}
}

void WriteEEPROM(uint8_t wrt){	
	HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_RESET);
	//HAL_GPIO_WritePin(GPIOA, OE_Pin, GPIO_PIN_SET);
	
	HAL_GPIO_WritePin(CE_GPIO_Port, CE_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(WE_GPIO_Port, WE_Pin, GPIO_PIN_SET);
	
	
	
	//HAL_GPIO_WritePin(GPIOA, Row0_Pin | Row1_Pin | Row2_Pin | Row3_Pin | Row4_Pin | Row5_Pin | Row6_Pin | Row7_Pin | Row8_Pin, GPIO_PIN_RESET);
	//HAL_GPIO_WritePin(GPIOC, Column0_Pin | Column1_Pin | Column2_Pin | Column3_Pin | Column4_Pin | Column5_Pin, GPIO_PIN_RESET);
	
	HAL_GPIO_WritePin(Row0_GPIO_Port, Row0_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row1_GPIO_Port, Row1_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row2_GPIO_Port, Row2_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row3_GPIO_Port, Row3_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row4_GPIO_Port, Row4_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row5_GPIO_Port, Row5_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row6_GPIO_Port, Row6_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row7_GPIO_Port, Row7_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Row8_GPIO_Port, Row8_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column0_GPIO_Port, Column0_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column1_GPIO_Port, Column1_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column2_GPIO_Port, Column2_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column3_GPIO_Port, Column3_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column4_GPIO_Port, Column4_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(Column5_GPIO_Port, Column5_Pin, GPIO_PIN_RESET);	
	
	if (row & 0b000000001)
		HAL_GPIO_WritePin(Row0_GPIO_Port, Row0_Pin, GPIO_PIN_SET);
	if (row & 0b000000010)
		HAL_GPIO_WritePin(Row1_GPIO_Port, Row1_Pin, GPIO_PIN_SET);
	if (row & 0b000000100)
		HAL_GPIO_WritePin(Row2_GPIO_Port, Row2_Pin, GPIO_PIN_SET);
	if (row & 0b000001000)
		HAL_GPIO_WritePin(Row3_GPIO_Port, Row3_Pin, GPIO_PIN_SET);
	if (row & 0b000010000)
		HAL_GPIO_WritePin(Row4_GPIO_Port, Row4_Pin, GPIO_PIN_SET);
	if (row & 0b000100000)
		HAL_GPIO_WritePin(Row5_GPIO_Port, Row5_Pin, GPIO_PIN_SET);
	if (row & 0b001000000)
		HAL_GPIO_WritePin(Row6_GPIO_Port, Row6_Pin, GPIO_PIN_SET);
	if (row & 0b010000000)
		HAL_GPIO_WritePin(Row7_GPIO_Port, Row7_Pin, GPIO_PIN_SET);
	if (row & 0b100000000)
		HAL_GPIO_WritePin(Row8_GPIO_Port, Row8_Pin, GPIO_PIN_SET);
	
	if (col & 0b000001)
		HAL_GPIO_WritePin(Column0_GPIO_Port, Column0_Pin, GPIO_PIN_SET);
	if (col & 0b000010)
		HAL_GPIO_WritePin(Column1_GPIO_Port, Column1_Pin, GPIO_PIN_SET);
	if (col & 0b000100)
		HAL_GPIO_WritePin(Column2_GPIO_Port, Column2_Pin, GPIO_PIN_SET);
	if (col & 0b001000)
		HAL_GPIO_WritePin(Column3_GPIO_Port, Column3_Pin, GPIO_PIN_SET);
	if (col & 0b010000)
		HAL_GPIO_WritePin(Column4_GPIO_Port, Column4_Pin, GPIO_PIN_SET);
	if (col & 0b100000)
		HAL_GPIO_WritePin(Column5_GPIO_Port, Column5_Pin, GPIO_PIN_SET);
		
	
	GPIO_InitTypeDef GPIO_InitStruct;
	GPIO_InitStruct.Pin = Data0_Pin|Data1_Pin|Data2_Pin|Data3_Pin
                          |Data4_Pin|Data5_Pin|Data6_Pin|Data7_Pin;
	GPIO_InitStruct.Mode = GPIO_MODE_OUTPUT_OD;
  GPIO_InitStruct.Pull = GPIO_NOPULL;
  GPIO_InitStruct.Speed = GPIO_SPEED_FREQ_LOW;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct);
	
	
	/*
	HAL_GPIO_WritePin(GPIOB, Data0_Pin, GPIO_PIN_SET);	
	HAL_GPIO_WritePin(GPIOB, Data1_Pin, GPIO_PIN_SET);	
  HAL_GPIO_WritePin(GPIOB, Data2_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data3_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data4_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data5_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data6_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data7_Pin, GPIO_PIN_SET);
	*/
	
	
	
	
	HAL_GPIO_WritePin(CE_GPIO_Port, CE_Pin, GPIO_PIN_RESET);
	HAL_GPIO_WritePin(WE_GPIO_Port, WE_Pin, GPIO_PIN_RESET);	
		
	HAL_GPIO_WritePin(GPIOB, Data0_Pin, GPIO_PIN_SET);	
	HAL_GPIO_WritePin(GPIOB, Data1_Pin, GPIO_PIN_SET);	
  HAL_GPIO_WritePin(GPIOB, Data2_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data3_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data4_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data5_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data6_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(GPIOB, Data7_Pin, GPIO_PIN_SET);
	
	ProgramDelay(10000);
	HAL_GPIO_WritePin(CE_GPIO_Port, CE_Pin, GPIO_PIN_SET);
	HAL_GPIO_WritePin(WE_GPIO_Port, WE_Pin, GPIO_PIN_SET);
	
	HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_RESET);

	//HAL_GPIO_WritePin(GPIOB, WE_Pin, GPIO_PIN_RESET);
	
	
	//HAL_GPIO_WritePin(GPIOA, CE_Pin, GPIO_PIN_RESET);
	
	//HAL_GPIO_WritePin(GPIOA, Row0_Pin | Row1_Pin | Row2_Pin | Row3_Pin | Row4_Pin | Row5_Pin | Row6_Pin | Row7_Pin | Row8_Pin, GPIO_PIN_RESET);
	//HAL_GPIO_WritePin(GPIOC, Column0_Pin | Column1_Pin | Column2_Pin | Column3_Pin | Column4_Pin | Column5_Pin, GPIO_PIN_RESET);
	//HAL_Delay(10);

	
	
	/*
	HAL_GPIO_WritePin(GPIOB, Data0_Pin, (wrt & 0b00000001));	
	HAL_GPIO_WritePin(GPIOB, Data1_Pin, (wrt & 0b00000010) >> 1);	
  HAL_GPIO_WritePin(GPIOB, Data2_Pin, (wrt & 0b00000100) >> 2);
	HAL_GPIO_WritePin(GPIOB, Data3_Pin, (wrt & 0b00001000) >> 3);
	HAL_GPIO_WritePin(GPIOB, Data4_Pin, (wrt & 0b00010000) >> 4);
	HAL_GPIO_WritePin(GPIOB, Data5_Pin, (wrt & 0b00100000) >> 5);
	HAL_GPIO_WritePin(GPIOB, Data6_Pin, (wrt & 0b01000000) >> 6);
	HAL_GPIO_WritePin(GPIOB, Data7_Pin, (wrt & 0b10000000) >> 7);*/
	
	
	//HAL_GPIO_WritePin(GPIOA, WE_Pin, GPIO_PIN_RESET);


	//HAL_GPIO_WritePin(GPIOB, WE_Pin | CE_Pin, GPIO_PIN_SET);
	
	
	GPIO_InitTypeDef GPIO_InitStruct_;
	GPIO_InitStruct_.Pin = Data0_Pin|Data1_Pin|Data2_Pin|Data3_Pin
                          |Data4_Pin|Data5_Pin|Data6_Pin|Data7_Pin;
  GPIO_InitStruct_.Mode = GPIO_MODE_INPUT;
  GPIO_InitStruct_.Pull = GPIO_NOPULL;
  HAL_GPIO_Init(GPIOB, &GPIO_InitStruct_);
}

void HAL_GPIO_EXTI_Callback(uint16_t GPIO_Pin)
{
	
  if(GPIO_Pin== BTN1_Pin) {
		//HAL_GPIO_WritePin(GPIOA, LD2_Pin, GPIO_PIN_SET);
		HAL_NVIC_DisableIRQ(EXTI15_10_IRQn);
		HAL_NVIC_DisableIRQ(EXTI9_5_IRQn);
		HAL_NVIC_DisableIRQ(EXTI4_IRQn);
		col--;
    if (col < 0){
			col = 63;
			row--;
			if (row < 0)
				row = 511;
		}
			ReadEEPROM();
			ShowData();
		
		flag_irq = 1;
    time_irq = HAL_GetTick();

  } else if (GPIO_Pin == BTN2_Pin){
		//HAL_GPIO_WritePin(GPIOA, LD2_Pin, GPIO_PIN_SET);
		HAL_NVIC_DisableIRQ(EXTI15_10_IRQn);
		HAL_NVIC_DisableIRQ(EXTI9_5_IRQn);
		HAL_NVIC_DisableIRQ(EXTI4_IRQn);
		
/*		for (int j = 0; j < 512*512; j++){
			   col++;
				if (col > 63){
					col = 0;
					row++;
					if (row > 511)
						row = 0;
				}
					ReadEEPROM();
				ProgramDelay(10000);
					//ShowData();
		}*/
		
		
    col++;
    if (col > 63){
			col = 0;
			row++;
			if (row > 511)
				row = 0;
		}
			ReadEEPROM();
			ShowData();
		
		flag_irq = 1;
    time_irq = HAL_GetTick();
  } else if (GPIO_Pin == B1_Pin){
		HAL_NVIC_DisableIRQ(EXTI15_10_IRQn);
		HAL_NVIC_DisableIRQ(EXTI9_5_IRQn);
		HAL_NVIC_DisableIRQ(EXTI4_IRQn);
		
		//HAL_GPIO_WritePin(GPIOA, LD2_Pin, GPIO_PIN_SET);
		WriteEEPROM(0b00000001);
		
		//HAL_GPIO_WritePin(GPIOA, WE_Pin, GPIO_PIN_SET);		
		//HAL_GPIO_WritePin(GPIOA, CE_Pin, GPIO_PIN_SET);
		//HAL_GPIO_WritePin(OE_GPIO_Port, OE_Pin, GPIO_PIN_SET);
		
		
		//HAL_GPIO_WritePin(Row1_GPIO_Port, Row0_Pin | Row1_Pin | Row2_Pin | Row3_Pin | Row4_Pin| Row5_Pin | Row6_Pin | Row7_Pin | Row8_Pin, GPIO_PIN_SET);
		//HAL_GPIO_WritePin(Column1_GPIO_Port, Column0_Pin | Column1_Pin | Column2_Pin | Column3_Pin | Column4_Pin| Column5_Pin, GPIO_PIN_SET);
		//HAL_GPIO_WritePin(Row1_GPIO_Port, Row1_Pin, GPIO_PIN_SET);
		
		
		flag_irq = 1;
    time_irq = HAL_GetTick();
	}



	
}
/* USER CODE END 4 */

/**
  * @brief  This function is executed in case of error occurrence.
  * @retval None
  */
void Error_Handler(void)
{
  /* USER CODE BEGIN Error_Handler_Debug */
  /* User can add his own implementation to report the HAL error return state */
  __disable_irq();
  while (1)
  {
  }
  /* USER CODE END Error_Handler_Debug */
}

#ifdef  USE_FULL_ASSERT
/**
  * @brief  Reports the name of the source file and the source line number
  *         where the assert_param error has occurred.
  * @param  file: pointer to the source file name
  * @param  line: assert_param error line source number
  * @retval None
  */
void assert_failed(uint8_t *file, uint32_t line)
{
  /* USER CODE BEGIN 6 */
  /* User can add his own implementation to report the file name and line number,
     ex: printf("Wrong parameters value: file %s on line %d\r\n", file, line) */
  /* USER CODE END 6 */
}
#endif /* USE_FULL_ASSERT */
