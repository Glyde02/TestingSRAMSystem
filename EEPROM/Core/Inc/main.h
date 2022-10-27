/* USER CODE BEGIN Header */
/**
  ******************************************************************************
  * @file           : main.h
  * @brief          : Header for main.c file.
  *                   This file contains the common defines of the application.
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

/* Define to prevent recursive inclusion -------------------------------------*/
#ifndef __MAIN_H
#define __MAIN_H

#ifdef __cplusplus
extern "C" {
#endif

/* Includes ------------------------------------------------------------------*/
#include "stm32f1xx_hal.h"

/* Private includes ----------------------------------------------------------*/
/* USER CODE BEGIN Includes */

/* USER CODE END Includes */

/* Exported types ------------------------------------------------------------*/
/* USER CODE BEGIN ET */

/* USER CODE END ET */

/* Exported constants --------------------------------------------------------*/
/* USER CODE BEGIN EC */

/* USER CODE END EC */

/* Exported macro ------------------------------------------------------------*/
/* USER CODE BEGIN EM */

/* USER CODE END EM */

/* Exported functions prototypes ---------------------------------------------*/
void Error_Handler(void);

/* USER CODE BEGIN EFP */

/* USER CODE END EFP */

/* Private defines -----------------------------------------------------------*/
#define B1_Pin GPIO_PIN_13
#define B1_GPIO_Port GPIOC
#define B1_EXTI_IRQn EXTI15_10_IRQn
#define Column0_Pin GPIO_PIN_0
#define Column0_GPIO_Port GPIOC
#define Column1_Pin GPIO_PIN_1
#define Column1_GPIO_Port GPIOC
#define Column2_Pin GPIO_PIN_2
#define Column2_GPIO_Port GPIOC
#define Column3_Pin GPIO_PIN_3
#define Column3_GPIO_Port GPIOC
#define CE_Pin GPIO_PIN_0
#define CE_GPIO_Port GPIOA
#define WE_Pin GPIO_PIN_1
#define WE_GPIO_Port GPIOA
#define BTN2_Pin GPIO_PIN_4
#define BTN2_GPIO_Port GPIOA
#define BTN2_EXTI_IRQn EXTI4_IRQn
#define LD2_Pin GPIO_PIN_5
#define LD2_GPIO_Port GPIOA
#define Row0_Pin GPIO_PIN_6
#define Row0_GPIO_Port GPIOA
#define Row1_Pin GPIO_PIN_7
#define Row1_GPIO_Port GPIOA
#define Column4_Pin GPIO_PIN_4
#define Column4_GPIO_Port GPIOC
#define Column5_Pin GPIO_PIN_5
#define Column5_GPIO_Port GPIOC
#define Data2_Pin GPIO_PIN_2
#define Data2_GPIO_Port GPIOB
#define Latch_Pin GPIO_PIN_10
#define Latch_GPIO_Port GPIOB
#define Data3_Pin GPIO_PIN_11
#define Data3_GPIO_Port GPIOB
#define Data4_Pin GPIO_PIN_12
#define Data4_GPIO_Port GPIOB
#define Data0_Pin GPIO_PIN_13
#define Data0_GPIO_Port GPIOB
#define Data1_Pin GPIO_PIN_14
#define Data1_GPIO_Port GPIOB
#define Data5_Pin GPIO_PIN_15
#define Data5_GPIO_Port GPIOB
#define OE_Pin GPIO_PIN_6
#define OE_GPIO_Port GPIOC
#define BTN1_Pin GPIO_PIN_7
#define BTN1_GPIO_Port GPIOC
#define BTN1_EXTI_IRQn EXTI9_5_IRQn
#define Row2_Pin GPIO_PIN_8
#define Row2_GPIO_Port GPIOA
#define Row3_Pin GPIO_PIN_9
#define Row3_GPIO_Port GPIOA
#define Row4_Pin GPIO_PIN_10
#define Row4_GPIO_Port GPIOA
#define Row5_Pin GPIO_PIN_11
#define Row5_GPIO_Port GPIOA
#define Row6_Pin GPIO_PIN_12
#define Row6_GPIO_Port GPIOA
#define Row7_Pin GPIO_PIN_10
#define Row7_GPIO_Port GPIOC
#define Row8_Pin GPIO_PIN_11
#define Row8_GPIO_Port GPIOC
#define Data6_Pin GPIO_PIN_6
#define Data6_GPIO_Port GPIOB
#define Data7_Pin GPIO_PIN_7
#define Data7_GPIO_Port GPIOB
#define Data_Pin GPIO_PIN_8
#define Data_GPIO_Port GPIOB
#define Shift_Pin GPIO_PIN_9
#define Shift_GPIO_Port GPIOB
/* USER CODE BEGIN Private defines */

/* USER CODE END Private defines */

#ifdef __cplusplus
}
#endif

#endif /* __MAIN_H */
